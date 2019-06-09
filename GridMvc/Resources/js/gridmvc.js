/***
* Grid.Mvc
* Examples and documentation at: http://gridmvc.codeplex.com
* Version: 3.0.0
* Requires: window.jQuery v1.3+
* LGPL license: http://gridmvc.codeplex.com/license
*/
window.pageGrids = window.pageGrids || {};
$.fn.extend({
    gridmvc: function () {
        var aObj = [];
        $(this).each(function () {
            if (!$(this).data("gridmvc")) {
                var options = { lang: $(this).attr("data-lang"), selectable: $(this).attr("data-selectable") == "true", multiplefilters: $(this).attr("data-multiplefilters") == "true", currentPage: $(this).find(".grid-pager").first().attr("data-currentpage") };
                var grid = new GridMvc(this, options);
                var name = $(this).attr("data-gridname");
                if (name.length > 0)
                    window.pageGrids[$(this).attr("data-gridname")] = grid;

                aObj.push(grid);
                $(this).data("gridmvc", grid);
            } else {
                aObj.push($(this).data("gridmvc"));
            }
        });
        if (aObj.length == 1)
            return aObj[0];
        return aObj;
    }
});

GridMvc = (function ($) {
    function gridMvc(container, options) {
        this.jqContainer = $(container);
        options = options || {};
        this.options = $.extend({}, this.defaults(), options);
        this.init();
    }

    gridMvc.prototype.init = function () {
        //load current lang options:
        this.lang = GridMvc.lang[this.options.lang];
        if (typeof (this.lang) == 'undefined')
            this.lang = GridMvc.lang.en;
        this.events = [];
        if (this.options.selectable)
            this.initGridRowsEvents();
        //Register default filter widgets:
        this.filterWidgets = [];
        this.addFilterWidget(new TextFilterWidget());
        this.addFilterWidget(new NumberFilterWidget());
        this.addFilterWidget(new DateTimeFilterWidget());
        this.addFilterWidget(new BooleanFilterWidget());

        this.openedMenuBtn = null;
        this.initFilters();
        this.initSearch();
    };
    /***
    * Handle Grid row events
    */
    gridMvc.prototype.initGridRowsEvents = function () {
        var $this = this;
        this.jqContainer.on("click", ".grid-row", function () {
            $this.rowClicked.call(this, $this);
        });
    };
    /***
    * Trigger on Grid row click
    */
    gridMvc.prototype.rowClicked = function ($context) {
        if (!$context.options.selectable)
            return;
        var row = $(this).closest(".grid-row");
        if (row.length <= 0)
            return;
        var gridRow = {};
        row.find(".grid-cell").each(function () {
            var columnName = $(this).attr("data-name");
            if (columnName.length > 0)
                gridRow[columnName] = $(this).text();
        });
        var evt = $.Event("RowClicked");
        $context.notifyOnRowSelect(gridRow, evt);
        if (!evt.isDefaultPrevented())
            $context.markRowSelected(row);
    };
    /***
    * Mark Grid row as selected
    */
    gridMvc.prototype.markRowSelected = function (row) {
        this.jqContainer.find(".grid-row.grid-row-selected").removeClass("grid-row-selected");
        row.addClass("grid-row-selected");
    };
    /***
    * Default Grid.Mvc options
    */
    gridMvc.prototype.defaults = function () {
        return {
            selectable: true,
            multiplefilters: false,
            lang: 'en'
        };
    };
    /***
    * ============= EVENTS =============
    * Methods that provides functionality for grid events
    */
    gridMvc.prototype.onRowSelect = function (func) {
        this.events.push({ name: "onRowSelect", callback: func });
    };

    gridMvc.prototype.notifyOnRowSelect = function (row, e) {
        e.row = row;
        this.notifyEvent("onRowSelect", e);
    };

    gridMvc.prototype.notifyEvent = function (eventName, e) {
        for (var i = 0; i < this.events.length; i++) {
            if (this.events[i].name == eventName)
                if (!this.events[i].callback(e)) break;
        }
    };
    /***
    * ============= FILTERS =============
    * Methods that provides functionality for filtering
    */
    /***
    * Method search all filter buttons and register 'onclick' event handlers:
    */
    gridMvc.prototype.initFilters = function () {
        var filterHtml = this.filterMenuHtml();
        var self = this;
        this.jqContainer.find(".grid-filter").each(function () {
            $(this).click(function () {
                return self.openFilterPopup.call(this, self, filterHtml);
            });
        });
    };
    /***
    * Shows filter popup window and render filter widget
    */
    gridMvc.prototype.openFilterPopup = function (self, html) {
        //retrive all column filter parameters from html attrs:
        var columnType = $(this).attr("data-type") || "";
        //determine widget
        var widget = self.getFilterWidgetForType(columnType);
        //if widget for specified column type not found - do nothing
        if (widget == null)
            return false;

        //if widget allready rendered - just open popup menu:
        if (this.hasAttribute("data-rendered")) {
            var or = self.openMenuOnClick.call(this, self);
            self.setupPopupInitialPosition($(this));
            if (!or && typeof (widget.onShow) != 'undefined')
                widget.onShow();
            return or;
        }

        var columnName = $(this).attr("data-name") || "";
        var filterData = $(this).attr("data-filterdata") || "";
        var widgetData = $(this).attr("data-widgetdata") || "{}";
        var filterDataObj = self.parseFilterValues(filterData) || {};
        var filterUrl = $(this).attr("data-url") || "";

        //mark filter as rendered
        $(this).attr("data-rendered", "1");
        //append base popup layout:
        $(this).append(html);

        //determine widget container:
        var widgetContainer = $(this).find(".grid-popup-widget");
        //onRender target widget
        if (typeof (widget.onRender) != 'undefined')
            widget.onRender(widgetContainer, self.lang, columnType, filterDataObj, function (values) {
                self.closeOpenedPopups();
                self.applyFilterValues(filterUrl, columnName, values, false);
            }, $.parseJSON(widgetData));
        //adding 'clear filter' button if needed:
        if ($(this).find(".grid-filter-btn").hasClass("filtered") && widget.showClearFilterButton()) {
            var inner = $(this).find(".grid-popup-additional");
            inner.append(self.getClearFilterButton(filterUrl));
            inner.find(".grid-filter-clear").click(function () {
                self.applyFilterValues(filterUrl, columnName, "", true);
            });
        }
        var openResult = self.openMenuOnClick.call(this, self);
        if (typeof (widget.onShow) != 'undefined')
            widget.onShow();
        self.setupPopupInitialPosition($(this));
        return openResult;
    };

    gridMvc.prototype.setupPopupInitialPosition = function (popup) {
        var drop = popup.find(".grid-dropdown");
        function getInfo() {
            var arrow = popup.find(".grid-dropdown-arrow");
            return { arrow: arrow, currentDropLeft: parseInt(drop.css("left")), currentArrowLeft: parseInt(arrow.css("left")) };
        }
        var dropLeft = drop.offset().left;
        if (dropLeft < 0) {
            var info = getInfo();
            info.arrow.css({ left: (info.currentArrowLeft + dropLeft - 10) + "px" });
            drop.css({ left: (info.currentDropLeft - dropLeft + 10) + "px" });
            return;
        }
        var dropWidth = drop.width();
        var offsetRight = $(window).width() + $(window).scrollLeft() - (dropLeft + dropWidth);
        if (offsetRight < 0) {
            var info = getInfo();
            info.arrow.css({ left: (info.currentArrowLeft - offsetRight + 5) + "px" });
            drop.css({ left: (info.currentDropLeft + offsetRight - 5) + "px" });
        }
    };
    /***
    * Returns layout of filter popup menu
    */
    gridMvc.prototype.filterMenuHtml = function () {
        return '<div class="dropdown dropdown-menu grid-dropdown" style="display: none;">\
                    <div class="grid-dropdown-arrow"></div>\
                    <div class="grid-dropdown-inner">\
                            <div class="grid-popup-widget"></div>\
                            <div class="grid-popup-additional"></div>\
                    </div>\
                </div>';
    };
    /***
    * Returns layout of 'clear filter' button
    */
    gridMvc.prototype.getClearFilterButton = function () {
        return '<ul class="menu-list">\
                    <li><a class="grid-filter-clear" href="javascript:void(0);">' + this.lang.clearFilterLabel + '</a></li>\
                </ul>';
    };
    /***
    * Register filter widget in widget collection
    */
    gridMvc.prototype.addFilterWidget = function (widget) {
        this.filterWidgets.push(widget);
    };
    /***
    * Parse filter settings from data attribute
    */
    gridMvc.prototype.parseFilterValues = function (filterData) {
        var opt = $.parseJSON(filterData);
        var filters = [];
        for (var i = 0; i < opt.length; i++) {
            filters.push({ filterValue: this.urldecode(opt[i].FilterValue), filterType: opt[i].FilterType, columnName: opt[i].ColumnName });
        }
        return filters;
    };

    gridMvc.prototype.urldecode = function (str) {
        return decodeURIComponent((str + '').replace(/\+/g, '%20'));
    };

    /***
    * Return registred widget for specific column type name
    */
    gridMvc.prototype.getFilterWidgetForType = function (typeName) {
        for (var i = 0; i < this.filterWidgets.length; i++) {
            if ($.inArray(typeName, this.filterWidgets[i].getAssociatedTypes()) >= 0)
                return this.filterWidgets[i];
        }
        return null;
    };
    /***
    * Replace existed filter widget
    */
    gridMvc.prototype.replaceFilterWidget = function (typeNameToReplace, widget) {
        for (var i = 0; i < this.filterWidgets.length; i++) {
            if ($.inArray(typeNameToReplace, this.filterWidgets[i].getAssociatedTypes()) >= 0) {
                this.filterWidgets.splice(i, 1);
                this.addFilterWidget(widget);
                return true;
            }
        }
        return false;
    };
    /***
    * Applies selected filter value by redirecting to another url:
    */
    gridMvc.prototype.applyFilterValues = function (initialUrl, columnName, values, skip) {
        var filters = this.jqContainer.find(".grid-filter");

        var initialFiltersString = this.jqContainer.attr("data-initfilters");
        var initialFilters;
        if (initialFiltersString) {
            initialFilters = initialFiltersString.split(',');
        }
        else {
            initialFilters = new Array();
        }
        var clearInitialFiltersString = this.jqContainer.find(".grid-filter").attr("data-clearinitfilter");
        var clearInitialFilters;
        if (clearInitialFiltersString) {
            clearInitialFilters = clearInitialFiltersString.split(',');
        }
        else {
            clearInitialFilters = new Array();
        }
        if (initialFilters.includes(columnName) && !clearInitialFilters.includes(columnName)) {
            clearInitialFilters.push(columnName);
        }

        if (initialUrl.length > 0)
            initialUrl += "&";

        var url = "";
        if (!skip) {
            url += this.getFilterQueryData(columnName, values);
        }

        if (this.options.multiplefilters) { //multiple filters enabled
            for (var i = 0; i < filters.length; i++) {
                if ($(filters[i]).attr("data-name") != columnName) {
                    var filterData = this.parseFilterValues($(filters[i]).attr("data-filterdata"));
                    if (filterData.length == 0) continue;
                    if (url.length > 0) url += "&";
                    url += this.getFilterQueryData($(filters[i]).attr("data-name"), filterData);
                } else {
                    continue;
                }
            }

            for (var j = 0; j < clearInitialFilters.length; j++) {
                if (url.length > 0) url += "&";
                url += "grid-clearinitfilter=" + clearInitialFilters[j];
            }
        }
        else {
            for (var k = 0; k < initialFilters.length; k++) {
                if (url.length > 0) url += "&";
                url += "grid-clearinitfilter=" + initialFilters[k];
            }
        }  

        window.location.search = initialUrl + url;
    };
    gridMvc.prototype.getFilterQueryData = function (columnName, values) {
        var url = "";
        for (var i = 0; i < values.length; i++) {
            url += "grid-filter=" + encodeURIComponent(columnName) + "__" + values[i].filterType + "__" + encodeURIComponent(values[i].filterValue);
            if (i != values.length - 1)
                url += "&";
        }
        return url;
    };
    /***
    * ============= POPUP MENU =============
    * Methods that provides base functionality for popup menus
    */
    gridMvc.prototype.openMenuOnClick = function (self) {
        if ($(this).hasClass("clicked")) return true;
        self.closeOpenedPopups();
        $(this).addClass("clicked");
        var popup = $(this).find(".dropdown-menu");
        if (popup.length == 0) return true;
        popup.show();
        popup.addClass("opened");
        self.openedMenuBtn = $(this);
        $(document).bind("click.gridmvc", function (e) {
            self.documentCallback(e, self);
        });
        return false;
    };

    gridMvc.prototype.documentCallback = function (e, $context) {
        e = e || event;
        var target = e.target || e.srcElement;
        var box = $(".dropdown-menu.opened").get(0);
        var html = $("html").get(0);
        if (typeof box != "undefined") {
            do {
                if (box == target) {
                    // Click occured inside the box, do nothing.
                    return;
                }
                if (html == target) {
                    box.style.display = "none";
                    $(box).removeClass("opened");
                }
                target = target.parentNode;
            } while (target); // Click was outside the box, hide it.

        }
        if ($context.openedMenuBtn != null)
            $context.openedMenuBtn.removeClass("clicked");
        $(document).unbind("click.gridmvc");
    };

    gridMvc.prototype.closeOpenedPopups = function () {
        var openedPopup = $(".dropdown-menu.opened");
        openedPopup.hide();
        openedPopup.removeClass("opened");
        if (this.openedMenuBtn != null)
            this.openedMenuBtn.removeClass("clicked");
    };

    /* Grid.Mvc clients functions */

    gridMvc.prototype.selectable = function (enable) {
        this.options.selectable = enable;
    };

    /***
    * ============= SEARCH =============
    * Methods that provides functionality for searching
    */
    /***
    * Method search all search buttons and register 'onclick' event handlers:
    */
    gridMvc.prototype.initSearch = function () {
        var self = this;
        this.jqContainer.find(".grid-search-apply").each(function () {
            $(this).click(function () {
                var searchText = self.jqContainer.find(".grid-search-input").first().val();
                self.applySearchValues(searchText, false);
            });
        });
        this.jqContainer.find(".grid-search-clear").each(function () {
            $(this).click(function () {
                self.applySearchValues("", true);
            });
        });
    };

    /***
    * Applies selected search value by redirecting to another url:
    */
    gridMvc.prototype.applySearchValues = function (searchText, skip) {      
        var url = "";
        var gridSearch = this.jqContainer.find(".grid-search").first();
        if (gridSearch) {
            url = gridSearch.attr("data-search-url") || "";
        }

        if (skip) {
            url = (url.length && url[0] == '?') ? url.slice(1) : url;
            var params = url.split('&');
            for (var i = params.length - 1; i >= 0; i--) {
                var param = params[i].split('=');
                if (param[0] === 'grid-search') {
                    params.splice(i, 1);
                }
            }
            url = params.join('&');
        }
        else {
            if (url.length > 0)
                url += "&";
            url += this.getSearchQueryData(searchText);
        } 
        window.location.search = url;
    };

    gridMvc.prototype.getSearchQueryData = function (searchText) {
        var url = "";
        if (searchText) {
            url += "grid-search=" + encodeURIComponent(searchText);
        }
        return url;
    };

    return gridMvc;
})(window.jQuery);

/***
* ============= LOCALIZATION =============
* You can localize Grid.Mvc by creating localization files and include it on the page after this script file
* This script file provides localization only for english language.
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/
if (typeof (GridMvc.lang) == 'undefined')
    GridMvc.lang = {};
GridMvc.lang.en = {
    filterTypeLabel: "Type: ",
    filterValueLabel: "Value:",
    applyFilterButtonText: "Apply",
    filterSelectTypes: {
        Equals: "Equals",
        StartsWith: "Starts with",
        Contains: "Contains",
        EndsWith: "Ends with",
        GreaterThan: "Greater than",
        LessThan: "Less than",
        GreaterThanOrEquals: "Greater than or equals",
        LessThanOrEquals: "Less than or equals"
    },
    code: 'en',
    boolTrueLabel: "Yes",
    boolFalseLabel: "No",
    clearFilterLabel: "Clear filter"
};
/***
* ============= FILTER WIDGETS =============
* Filter widget allows onRender custom filter user interface for different columns. 
* For example if your added column is of type "DateTime" - widget can onRender calendar for picking filter value.
* This script provider base widget for default .Net types: System.String, System.Int32, System.Decimal etc.
* If you want to provide custom filter functionality - you can assign your own widget type for column and write widget for this types.
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/

/***
* TextFilterWidget - Provides filter interface for text columns (of type "System.String")
* This widget onRenders filter interface with conditions, which specific for text types: contains, starts with etc.
*/
TextFilterWidget = (function ($) {
    function textFilterWidget() { }
    /***
    * This method must return type of columns that must be associated with current widget
    * If you not specify your own type name for column (see 'SetFilterWidgetType' method), GridMvc setup column type name from .Net type ("System.DateTime etc.)
    */
    textFilterWidget.prototype.getAssociatedTypes = function () { return ["System.String"]; };
    /***
    * This method invokes when filter widget was shown on the page
    */
    textFilterWidget.prototype.onShow = function () {
        var textBox = this.container.find(".grid-filter-input");
        if (textBox.length <= 0) return;
        textBox.focus();
    };
    /***
    * This method specify whether onRender 'Clear filter' button for this widget.
    */
    textFilterWidget.prototype.showClearFilterButton = function () { return true; };
    /***
    * This method will invoke when user first clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * filterType - current filter type (like equals, starts with etc);
    * filterValue - current filter value;
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    */
    textFilterWidget.prototype.onRender = function (container, lang, typeName, values, cb) {
        this.cb = cb;
        this.container = container;
        this.lang = lang;
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };//support only one filter value
        this.renderWidget();
        this.registerEvents();
    };
    /***
    * Internal method that build widget layout and append it to the widget container
    */
    textFilterWidget.prototype.renderWidget = function () {
        var html = '<div class="form-group">\
                        <label>' + this.lang.filterTypeLabel + '</label>\
                        <select class="grid-filter-type form-control">\
                            <option value="1" ' + (this.value.filterType == "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                            <option value="2" ' + (this.value.filterType == "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Contains + '</option>\
                            <option value="3" ' + (this.value.filterType == "3" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.StartsWith + '</option>\
                            <option value="4" ' + (this.value.filterType == "4" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.EndsWith + '</option>\
                        </select>\
                    </div>\
                    <div class="form-group">\
                        <label>' + this.lang.filterValueLabel + '</label>\
                        <input type="text" class="grid-filter-input form-control" value="' + this.value.filterValue + '" />\
                    </div>\
                    <div class="grid-filter-buttons">\
                        <button type="button" class="btn btn-primary grid-apply" >' + this.lang.applyFilterButtonText + '</button>\
                    </div>';
        this.container.append(html);
    };
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    textFilterWidget.prototype.registerEvents = function () {
        //get apply button from:
        var applyBtn = this.container.find(".grid-apply");
        //save current context:
        var $context = this;
        //register onclick event handler
        applyBtn.click(function () {
            //get selected filter type:
            var type = $context.container.find(".grid-filter-type").val();
            //get filter value:
            var value = $context.container.find(".grid-filter-input").val();
            //invoke callback with selected filter values:
            var filterValues = [{ filterType: type, filterValue: value }];
            $context.cb(filterValues);
        });
        //register onEnter event for filter text box:
        this.container.find(".grid-filter-input").keyup(function (event) {
            if (event.keyCode == 13) { applyBtn.click(); }
            if (event.keyCode == 27) { GridMvc.closeOpenedPopups(); }
        });
    };

    return textFilterWidget;
})(window.jQuery);

/***
* NumberFilterWidget - Provides filter interface for number columns
* This widget onRenders filter interface with conditions, which specific for number types: great than, less that etc.
* Also validates user's input for digits
*/
NumberFilterWidget = (function ($) {

    function numberFilterWidget() { }

    numberFilterWidget.prototype.showClearFilterButton = function () { return true; };

    numberFilterWidget.prototype.getAssociatedTypes = function () {
        return ["System.Int32", "System.Double", "System.Decimal", "System.Byte", "System.Single", "System.Float", "System.Int64", "System.Int16", "System.UInt64", "System.UInt32", "System.UInt16"];
    };

    numberFilterWidget.prototype.onShow = function () {
        var textBox = this.container.find(".grid-filter-input");
        if (textBox.length <= 0) return; 
        textBox.focus();
    };

    numberFilterWidget.prototype.onRender = function (container, lang, typeName, values, cb) {
        this.cb = cb;
        this.container = container;
        this.lang = lang;
        this.typeName = typeName;
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };//support only one filter value
        this.renderWidget();
        this.registerEvents();
    };

    numberFilterWidget.prototype.renderWidget = function () {
        var html = '<div class="form-group">\
                        <label>' + this.lang.filterTypeLabel + '</label>\
                        <select class="grid-filter-type form-control">\
                            <option value="1" ' + (this.value.filterType == "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                            <option value="5" ' + (this.value.filterType == "5" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThan + '</option>\
                            <option value="6" ' + (this.value.filterType == "6" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThan + '</option>\
                        </select>\
                    </div>\
                    <div class="form-group">\
                        <label>' + this.lang.filterValueLabel + '</label>\
                        <input type="text" class="grid-filter-input form-control" value="' + this.value.filterValue + '" />\
                    </div>\
                    <div class="grid-filter-buttons">\
                        <button type="button" class="btn btn-primary grid-apply">' + this.lang.applyFilterButtonText + '</button>\
                    </div>';
        this.container.append(html);
    };

    numberFilterWidget.prototype.registerEvents = function () {
        var $context = this;
        var applyBtn = this.container.find(".grid-apply");
        applyBtn.click(function () {
            var type = $context.container.find(".grid-filter-type").val();
            var value = $context.container.find(".grid-filter-input").val();
            var filters = [{ filterType: type, filterValue: value }];
            $context.cb(filters);
        });
        var txt = this.container.find(".grid-filter-input");
        txt.keyup(function (event) {
            if (event.keyCode == 13) { applyBtn.click(); }
            if (event.keyCode == 27) { GridMvc.closeOpenedPopups(); }
        })
            .keypress(function (event) { return $context.validateInput.call($context, event); });
        if (this.typeName == "System.Byte")
            txt.attr("maxlength", "3");
    };

    numberFilterWidget.prototype.validateInput = function (evt) {
        var $event = evt || window.event;
        var key = $event.keyCode || $event.which;
        key = String.fromCharCode(key);
        var regex;
        switch (this.typeName) {
            case "System.Byte":
            case "System.Int32":
            case "System.Int64":
            case "System.UInt32":
            case "System.UInt64":
                regex = /[0-9]/;
                break;
            default:
                regex = /[0-9]|\.|\,/;
        }
        if (!regex.test(key)) {
            $event.returnValue = false;
            if ($event.preventDefault) $event.preventDefault();
        }
    };

    return numberFilterWidget;
})(window.jQuery);

/***
* DateTimeFilterWidget - Provides filter interface for date columns (of type "System.DateTime").
* If datepicker script included, this widget will render calendar for pick filter values
* In other case he onRender textbox field for specifing date value (more info at http://window.jQueryui.com/)
*/
DateTimeFilterWidget = (function ($) {

    function dateTimeFilterWidget() { }

    dateTimeFilterWidget.prototype.getAssociatedTypes = function () { return ["System.DateTime", "System.Date", "System.DateTimeOffset"]; };

    dateTimeFilterWidget.prototype.showClearFilterButton = function () { return true; };

    dateTimeFilterWidget.prototype.onRender = function (container, lang, typeName, values, applycb, data) {
        this.datePickerIncluded = typeof ($.fn.datepicker) != 'undefined';
        this.cb = applycb;
        this.data = data;
        this.typeName = typeName;
        this.container = container;
        this.lang = lang;
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };//support only one filter value
        this.renderWidget();
        this.registerEvents();
    };

    dateTimeFilterWidget.prototype.renderWidget = function () {
        var html = '<div class="form-group">\
                        <label>' + this.lang.filterTypeLabel + '</label>\
                        <select class="grid-filter-type form-control">\
                            <option value="1" ' + (this.value.filterType == "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                            <option value="5" ' + (this.value.filterType == "5" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThan + '</option>\
                            <option value="6" ' + (this.value.filterType == "6" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThan + '</option>\
                        </select>\
                    </div>' +
                        (this.datePickerIncluded ?
                            '<div class="grid-filter-datepicker"></div>'
                            :
                            '<div class="form-group">\
                                <label>' + this.lang.filterValueLabel + '</label>\
                                <input type="text" class="grid-filter-input form-control" value="' + this.value.filterValue + '" />\
                             </div>\
                             <div class="grid-filter-buttons">\
                                <input type="button" class="btn btn-primary grid-apply" value="' + this.lang.applyFilterButtonText + '" />\
                             </div>');
        this.container.append(html);
        //if window.jQueryUi included:
        if (this.datePickerIncluded) {
            var datePickerOptions = this.data || {};
            datePickerOptions.format = datePickerOptions.format || "yyyy-mm-dd";
            datePickerOptions.language = datePickerOptions.language || this.lang.code;

            var $context = this;
            var dateContainer = this.container.find(".grid-filter-datepicker");
            dateContainer.datepicker(datePickerOptions).on('changeDate', function (ev) {
                var type = $context.container.find(".grid-filter-type").val();
                //if (type == "1") {
                //    var tomorrow = new Date(ev.getTime());
                //    tomorrow.setDate(ev.getDate() + 1);
                //    var filterValues = [{ filterType: type, filterValue: ev.format() }];
                //}
                //else{
                    var filterValues = [{ filterType: type, filterValue: ev.format() }];
                //}
                $context.cb(filterValues);
            });
            if (this.value.filterValue)
                dateContainer.datepicker('update', this.value.filterValue);
        }
    };

    dateTimeFilterWidget.prototype.registerEvents = function () {
        var $context = this;
        var applyBtn = this.container.find(".grid-apply");
        applyBtn.click(function () {
            var type = $context.container.find(".grid-filter-type").val();
            var value = $context.container.find(".grid-filter-input").val();
            var filterValues = [{ filterType: type, filterValue: value }];
            $context.cb(filterValues);
        });
        this.container.find(".grid-filter-input").keyup(function (event) {
            if (event.keyCode == 13) {
                applyBtn.click();
            }
        });
    };

    return dateTimeFilterWidget;
})(window.jQuery);

/***
* BooleanFilterWidget - Provides filter interface for boolean columns (of type "System.Boolean").
* Renders two button for filter - true and false
*/
BooleanFilterWidget = (function ($) {

    function booleanFilterWidget() { }

    booleanFilterWidget.prototype.getAssociatedTypes = function () { return ["System.Boolean"]; };

    booleanFilterWidget.prototype.showClearFilterButton = function () { return true; };

    booleanFilterWidget.prototype.onRender = function (container, lang, typeName, values, cb) {
        this.cb = cb;
        this.container = container;
        this.lang = lang;
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };//support only one filter value
        this.renderWidget();
        this.registerEvents();
    };

    booleanFilterWidget.prototype.renderWidget = function () {
        var html = '<label>' + this.lang.filterValueLabel + '</label>\
                    <ul class="menu-list">\
                        <li><a class="grid-filter-choose ' + (this.value.filterValue == "true" ? "choose-selected" : "") + '" data-value="true" href="javascript:void(0);">' + this.lang.boolTrueLabel + '</a></li>\
                        <li><a class="grid-filter-choose ' + (this.value.filterValue == "false" ? "choose-selected" : "") + '" data-value="false" href="javascript:void(0);">' + this.lang.boolFalseLabel + '</a></li>\
                    </ul>';
        this.container.append(html);
    };

    booleanFilterWidget.prototype.registerEvents = function () { 
        var $context = this;
        var applyBtn = this.container.find(".grid-filter-choose");
        applyBtn.click(function () {
            var filterValues = [{ filterType: "1", filterValue: $(this).attr("data-value") }];
            $context.cb(filterValues);
        });
    };

    return booleanFilterWidget;
})(window.jQuery);

//startup init:
(function ($) {
    if (!$) return;//jquery not referenced
    $(function () {
        $(".grid-mvc").each(function () {
            $(".grid-mvc").gridmvc();
        });
    });
})(window.jQuery);