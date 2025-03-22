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
                var options = { lang: $(this).attr("data-lang"), selectable: $(this).attr("data-selectable") === "true", extsortable: $(this).attr("data-extsortable") === "true", multiplefilters: $(this).attr("data-multiplefilters") === "true", currentPage: $(this).find(".grid-pager").first().attr("data-currentpage"), dir: $(this).attr("dir") };
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
        if (aObj.length === 1)
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
        if (typeof this.lang === 'undefined')
            this.lang = GridMvc.lang.en;
        this.events = [];
        if (this.options.selectable)
            this.initGridRowsEvents();
        //Register default filter widgets:
        this.filterWidgets = [];
        this.addFilterWidget(new TextFilterWidget());
        this.addFilterWidget(new NumberFilterWidget());
        this.addFilterWidget(new DateTimeFilterWidget());
        this.addFilterWidget(new TimeOnlyFilterWidget());
        this.addFilterWidget(new BooleanFilterWidget());
        this.addFilterWidget(new ListFilterWidget());

        this.openedMenuBtn = null;
        this.initFilters();
        this.initSearch();
        this.initExtSort();
        this.initGroup();
        this.initChangePageSize();
        this.initGotoPage();
        this.initSync();
        this.initRemoveAllFilters();
    };
    //
    // Handle Grid row events
    
    gridMvc.prototype.initGridRowsEvents = function () {
        var $this = this;
        this.jqContainer.on("click", ".grid-row", function () {
            $this.rowClicked.call(this, $this);
        });
    };
    //
    // Trigger on Grid row click
    //
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
    //
    // Mark Grid row as selected
    //
    gridMvc.prototype.markRowSelected = function (row) {
        this.jqContainer.find(".grid-row.grid-row-selected").removeClass("grid-row-selected");
        row.addClass("grid-row-selected");
    };
    //
    // Default Grid.Mvc options
    //
    gridMvc.prototype.defaults = function () {
        return {
            selectable: true,
            multiplefilters: false,
            lang: 'en'
        };
    };
    //
    // ============= EVENTS =============
    // Methods that provides functionality for grid events
    //
    gridMvc.prototype.onRowSelect = function (func) {
        this.events.push({ name: "onRowSelect", callback: func });
    };

    gridMvc.prototype.notifyOnRowSelect = function (row, e) {
        e.row = row;
        this.notifyEvent("onRowSelect", e);
    };

    gridMvc.prototype.notifyEvent = function (eventName, e) {
        for (var i = 0; i < this.events.length; i++) {
            if (this.events[i].name === eventName)
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
    //
    // Shows filter popup window and render filter widget
    //
    gridMvc.prototype.openFilterPopup = function (self, html) {
        //retrive all column filter parameters from html attrs:
        var columnType = $(this).attr("data-type") || "";
        var isNullable = $(this).attr("data-isnullable") || "";
        //determine widget
        var widget = self.getFilterWidgetForType(columnType);
        //if widget for specified column type not found - do nothing
        if (widget === null)
            return false;

        var columnName = $(this).attr("data-name") || "";

        //if widget allready rendered - just open popup menu:
        if (this.hasAttribute("data-rendered")) {
            var or = self.openMenuOnClick.call(this, self);
            self.setupPopupInitialPosition($(this));
            if (!or && typeof widget.onShow !== 'undefined')
                widget.onShow(columnName);
            return or;
        }
        
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
        if (typeof widget.onRender !== 'undefined')
            widget.onRender(widgetContainer, self.lang, columnType, columnName, isNullable, filterDataObj, function (values) {
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
        if (typeof widget.onShow !== 'undefined')
            widget.onShow(columnName);
        self.setupPopupInitialPosition($(this));
        return openResult;
    };

    gridMvc.prototype.setupPopupInitialPosition = function (popup) {
        var drop = popup.find(".grid-dropdown");
        var table = drop.closest(".grid-table");
        if (this.options.dir === "rtl") {
            function getInfo() {
                var arrow = popup.find(".grid-dropdown-arrow");
                return { arrow: arrow, currentDropLeft: parseInt(drop.css("left")), currentArrowLeft: parseInt(arrow.css("left")) };
            }
            var dropLeft = drop.offset().left;
            var dropWidth = drop.width();
            var tableParentLeft = table.parent().offset().left;
            var tableParentWidth = table.parent().width();
            var tableScrollLeft = table.scrollLeft();
            var offsetRight = tableParentLeft + tableParentWidth - (dropLeft + dropWidth);
            var info;
            if (offsetRight < 0) {
                info = getInfo();
                info.arrow.css({ left: Math.trunc(info.currentArrowLeft - offsetRight) + "px" });
                drop.css({ left: Math.trunc(info.currentDropLeft + offsetRight) + "px" });
                return;
            }
            var offsetLeft = dropLeft - tableParentLeft;
            if (offsetLeft < 0) {
                info = getInfo();
                info.arrow.css({ left: Math.trunc(info.currentArrowLeft + offsetLeft) + "px" });
                drop.css({ left: Math.trunc(info.currentDropLeft - offsetLeft) + "px" });
            }
        }
        else {
            function getInfo() {
                var arrow = popup.find(".grid-dropdown-arrow");
                return { arrow: arrow, currentDropLeft: parseInt(drop.css("left")), currentArrowLeft: parseInt(arrow.css("left")) };
            }
            var dropLeft = drop.offset().left;
            var info;
            if (dropLeft < 0) {
                info = getInfo();
                info.arrow.css({ left: Math.trunc(info.currentArrowLeft + dropLeft - 10) + "px" });
                drop.css({ left: Math.trunc(info.currentDropLeft - dropLeft + 10) + "px" });
                return;
            }
            var dropWidth = drop.width();
            var tableParentLeft = table.parent().offset().left;
            var tableParentWidth = table.parent().width();
            var tableScrollLeft = table.scrollLeft();
            var offsetRight = tableParentLeft + tableParentWidth + tableScrollLeft - (dropLeft + dropWidth);
            if (offsetRight < 0) {
                info = getInfo();
                info.arrow.css({ left: Math.trunc(info.currentArrowLeft - offsetRight + 5) + "px" });
                drop.css({ left: Math.trunc(info.currentDropLeft + offsetRight - 5) + "px" });
            }
        }
    };
    //
    // Returns layout of filter popup menu
    //
    gridMvc.prototype.filterMenuHtml = function () {
        return '<div class="dropdown dropdown-menu grid-dropdown" style="display: none; position: relative;">\
                    <div class="grid-dropdown-arrow"></div>\
                    <div class="grid-dropdown-inner">\
                            <div class="grid-popup-widget"></div>\
                            <div class="grid-popup-additional"></div>\
                    </div>\
                </div>';
    };
    //
    // Returns layout of 'clear filter' button
    //
    gridMvc.prototype.getClearFilterButton = function () {
        return '<ul class="menu-list">\
                    <li><a class="grid-filter-clear" href="javascript:void(0);">' + this.lang.clearFilterLabel + '</a></li>\
                </ul>';
    };
    //
    // Register filter widget in widget collection
    //
    gridMvc.prototype.addFilterWidget = function (widget) {
        this.filterWidgets.push(widget);
    };
    //
    // Parse filter settings from data attribute
    //
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

    //
    // Return registred widget for specific column type name
    //
    gridMvc.prototype.getFilterWidgetForType = function (typeName) {
        for (var i = 0; i < this.filterWidgets.length; i++) {
            if ($.inArray(typeName, this.filterWidgets[i].getAssociatedTypes()) >= 0)
                return this.filterWidgets[i];
        }
        return null;
    };
    //
    // Replace existed filter widget
    //
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
    //
    // Applies selected filter value by redirecting to another url:
    //
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
                if ($(filters[i]).attr("data-name") !== columnName) {
                    var filterData = this.parseFilterValues($(filters[i]).attr("data-filterdata"));
                    if (filterData.length === 0) continue;
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
            if (i !== values.length - 1)
                url += "&";
        }
        return url;
    };
    //
    // ============= POPUP MENU =============
    // Methods that provides base functionality for popup menus
    //
    gridMvc.prototype.openMenuOnClick = function (self) {
        if ($(this).hasClass("clicked")) return true;
        self.closeOpenedPopups();
        $(this).addClass("clicked");
        var popup = $(this).find(".dropdown-menu");
        if (popup.length === 0) return true;
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
        if (typeof box !== "undefined") {
            do {
                if (box === target) {
                    // Click occured inside the box, do nothing.
                    return;
                }
                if (html === target) {
                    box.style.display = "none";
                    $(box).removeClass("opened");
                }
                target = target.parentNode;
            } while (target); // Click was outside the box, hide it.

        }
        if ($context.openedMenuBtn !== null)
            $context.openedMenuBtn.removeClass("clicked");
        $(document).unbind("click.gridmvc");
    };

    gridMvc.prototype.closeOpenedPopups = function () {
        var openedPopup = $(".dropdown-menu.opened");
        openedPopup.hide();
        openedPopup.removeClass("opened");
        if (this.openedMenuBtn !== null)
            this.openedMenuBtn.removeClass("clicked");
    };

    // Grid.Mvc clients functions 

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
        this.jqContainer.find(".grid-search-input").each(function () {
            $(this).keyup(function (e) {
                if (e.keyCode === 13) {
                    var searchText = $(this).val();
                    self.applySearchValues(searchText, false);
                }
            });
        });
        this.jqContainer.find(".grid-search-clear").each(function () {
            $(this).click(function () {
                self.applySearchValues("", true);
            });
        });
    };

    //
    // Applies selected search value by redirecting to another url:
    //
    gridMvc.prototype.applySearchValues = function (searchText, skip) {
        var url = "";
        var gridSearch = this.jqContainer.find(".grid-search").first();
        if (gridSearch) {
            url = gridSearch.attr("data-search-url") || "";
        }

        if (skip) {
            url = url.length && url[0] === '?' ? url.slice(1) : url;
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

    /***
    * ============= EXT SORTING =============
    * Methods that provides functionality for extended sorting
    */
    /***
    * Method search grid-extsort elements and register event handlers:
    */
    gridMvc.prototype.initExtSort = function () {
        var self = this;
        this.jqContainer.find(".grid-extsort-draggable").each(function () {
            $(this).on('dragstart', function (e) {
                e.originalEvent.dataTransfer.setData("text", e.target.text);
                e.originalEvent.dataTransfer.setData("column", e.target.dataset.column);
                if (e.target.dataset.sorted)
                    e.originalEvent.dataTransfer.setData("sorted", e.target.dataset.sorted);
            });
        });
        this.jqContainer.find(".grid-extsort-droppable").each(function () {
            $(this).on('dragenter', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $(this).addClass('folderDragOver');
            });

            $(this).on('dragover', function (e) {
                e.preventDefault();
                e.stopPropagation();
                e.originalEvent.dataTransfer.dropEffect = 'move';
            });

            $(this).on('dragleave', function (e) {
                $(this).removeClass('folderDragOver');
            });

            $(this).on('drop', function (e) {
                $(this).removeClass('folderDragOver');
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                var columnName = e.originalEvent.dataTransfer.getData("column");
                var direction = e.originalEvent.dataTransfer.getData("sorted") === "desc" ? 1 : 0; 
                if (columnName && columnName !== "undefined")
                    self.applyExtSortValues(columnName, direction, 1);
            });
        });
    };

    //
    // Applies selected ext sort values by redirecting to another url:
    // operation values: 0 - remove, 1 - add, 2 - change
    gridMvc.prototype.applyExtSortValues = function (columnName, direction, operation) {
        var url = "";
        var id;
        var gridExtSort = this.jqContainer.find(".grid-extsort-droppable").first();      
        if (gridExtSort) {
            url = gridExtSort.attr("data-extsort-url") || "";
            // return if column is already present in extended sorting
            var extSortStr = "grid-sorting=" + columnName;
            var regex = new RegExp(extSortStr, 'g');
            id = url.match(regex);
            if (id)
                return;
            // pos is count of "grid-sorting" words in the url 
            id = url.match(/grid-sorting=/g);
        }
        if (url.length > 0)
            url += "&";
        if (id)
            id = id.length + 1;
        else
            id = 1;
        url += this.getExtSortQueryData(columnName, direction, id);
        window.location.search = url;
    };

    gridMvc.prototype.getExtSortQueryData = function (column, sorted, pos) {
        var url = "";
        if (column && column !== "undefined") {
            url += "grid-sorting=" + encodeURIComponent(column) + "__" + sorted + "__" + pos;
        }
        return url;
    };

    /***
    * ============= GROUPING =============
    * Methods that provides functionality for grouping
    */
    /***
    * Method search grid-group elements and register event handlers:
    */
    gridMvc.prototype.initGroup = function () {
        var self = this;
        this.jqContainer.find(".grid-group").each(function () {
            $(this).click(function (e) {
                e.stopPropagation();
                var groupId = $(this).attr("data-group-id");

                if ($(this).hasClass("grid-group-caret")) {
                    $(this).removeClass("grid-group-caret");
                    $(this).addClass("grid-group-caret-down");
                    self.jqContainer.find("[data-group-row-id=" + groupId + "]").show();
                    self.jqContainer.find("[data-group-subrow-id=" + groupId + "]").each(function () {
                        var subgridRow = $(this);
                        var subgrid = subgridRow.prev().children(".grid-subgrid").first();
                        if (subgrid && subgrid.attr("data-is-visible") === "true") {
                            subgridRow.show();
                        }
                    });
                    self.jqContainer.find("[data-group-parent-id=" + groupId + "]").each(function () {
                        $(this).show();
                        $(this).find(".grid-group").each(function () {
                            if ($(this).hasClass("grid-group-caret")) {
                                $(this).trigger("click");
                            }
                        });
                    });
                }
                else if ($(this).hasClass("grid-group-caret-down")) {
                    $(this).removeClass("grid-group-caret-down");
                    $(this).addClass("grid-group-caret");
                    self.jqContainer.find("[data-group-row-id=" + groupId + "]").hide();
                    self.jqContainer.find("[data-group-subrow-id=" + groupId + "]").hide();
                    self.jqContainer.find("[data-group-parent-id=" + groupId + "]").each(function () {
                        $(this).hide();
                        $(this).find(".grid-group").each(function () {
                            if ($(this).hasClass("grid-group-caret-down")) {
                                $(this).trigger("click");
                            }
                        });
                    });
                }
            });
        });     
    };

    /***
    * ============= CHANGE PAGE SIZE =============
    * Methods that provides functionality for changing page size
    */
    /***
    * Method search all grid-change-page-size input elements and register 'keydown' event handlers:
    */
    gridMvc.prototype.initChangePageSize = function () {
        var self = this;
        this.jqContainer.find(".grid-change-page-size-input").each(function () {
            $(this).keydown(function (e) {
                if (e.keyCode === 9 || e.keyCode === 13) {
                    var pageSize = $(this).val();
                    var x = parseInt(pageSize, 10);
                    if (x > 0) {
                        self.changePageSize(pageSize);
                    }
                    else {
                        $(this).val(this.defaultValue);
                    }
                }
            });
        });
    };

    //
    // Applies selected pageSize value by redirecting to another url:
    //
    gridMvc.prototype.changePageSize = function (pageSize) {
        var x = parseInt(pageSize, 10);
        if (x <= 0) {
            return;
        }
        var url = "";
        var gridChangePageSize = this.jqContainer.find(".grid-change-page-size").first();
        if (gridChangePageSize) {
            url = gridChangePageSize.attr("data-change-page-size-url") || "";
        }

        if (url.length > 0)
            url += "&";
        url += this.getPageSizeQueryData(pageSize);
        
        window.location.search = url;
    };

    gridMvc.prototype.getPageSizeQueryData = function (pageSize) {
        var url = "";
        if (pageSize) {
            url += "grid-pagesize=" + encodeURIComponent(pageSize);
        }
        return url;
    };

    /***
    * ============= GO TO PAGE =============
    * Methods that provides functionality for going to a page
    */
    /***
    * Method search all grid-goto-page-input input elements and register 'keydown' event handlers:
    */
    gridMvc.prototype.initGotoPage = function () {
        var self = this;
        this.jqContainer.find(".grid-goto-page-input").each(function () {
            $(this).keydown(function (e) {
                if (e.keyCode === 9 || e.keyCode === 13) {
                    var page = $(this).val();
                    var x = parseInt(page, 10);
                    if (x > 0) {
                        self.goTo(page);
                    }
                    else {
                        $(this).val(this.defaultValue);
                    }
                }
            });
        });
    };

    //
    // Applies selected page number by redirecting to another url:
    //
    gridMvc.prototype.goTo = function (page) {
        var x = parseInt(page, 10);
        if (x <= 0) {
            return;
        }
        var url = "";
        var gridGotoPage = this.jqContainer.find(".grid-goto-page").first();
        if (gridGotoPage) {
            url = gridGotoPage.attr("data-goto-page-url") || "";
        }

        if (url.length > 0)
            url += "&";
        url += this.getGotoPageQueryData(page);

        window.location.search = url;
    };

    gridMvc.prototype.getGotoPageQueryData = function (page) {
        var url = "";
        if (page) {
            url += "grid-page=" + encodeURIComponent(page);
        }
        return url;
    };

    /***
    * ============= SYNC BUTTON =============
    * Methods that provides functionality for refresh a grid
    */
    /***
    * Method search all buttons and register 'onclick' event handlers:
    */
    gridMvc.prototype.initSync = function () {
        var self = this;
        this.jqContainer.find(".grid-button-sync").each(function () {
            $(this).click(function () {
                self.sync();
            });
        });
    };

    //
    // Applies selected pageSize value by redirecting to another url:
    //
    gridMvc.prototype.sync = function () {
        var url = "";
        var gridSync = this.jqContainer.find(".grid-button-sync").first();
        if (gridSync) {
            url = gridSync.attr("data-sync-url") || "";
        }
        window.location.search = url;
    };

    /***
    * ============= REMOVE ALL FILTERS =============
    * Methods that provides functionality for removing all filters
    */
    /***
    * Method search all buttons and register 'onclick' event handlers:
    */
    gridMvc.prototype.initRemoveAllFilters = function () {
        var self = this;
        this.jqContainer.find(".grid-button-all-filters-clear").each(function () {
            $(this).click(function () {
                self.removeAllFilters();
            });
        });
    };

    //
    // Applies selected pageSize value by redirecting to another url:
    //
    gridMvc.prototype.removeAllFilters = function () {
        var url = "";
        var gridAllFiltersClear = this.jqContainer.find(".grid-button-all-filters-clear").first();
        if (gridAllFiltersClear) {
            url = gridAllFiltersClear.attr("data-all-filters-clear-url") || "";
        }
        window.location.search = url;
    };

    return gridMvc;
})(window.jQuery);

/***
* ============= LOCALIZATION =============
* You can localize Grid.Mvc by creating localization files and include it on the page after this script file
* This script file provides localization only for english language.
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/
if (typeof GridMvc.lang === 'undefined')
    GridMvc.lang = {};
GridMvc.lang.en = {
    filterTypeLabel: "Type: ",
    filterValueLabel: "Value:",
    applyFilterButtonText: "Apply",
    filterSelectTypes: {
        Equals: "Equals",
        NotEquals: "Not equals",
        StartsWith: "Starts with",
        Contains: "Contains",
        EndsWith: "Ends with",
        GreaterThan: "Greater than",
        LessThan: "Less than",
        GreaterThanOrEquals: "Greater than or equals",
        LessThanOrEquals: "Less than or equals",
        And: "And",
        Or: "Or",
        IsNull: "Is null",
        IsNotNull: "Is not null",
        Duplicated: "Duplicated",
        NotDuplicated: "Not duplicated"
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
    //
    // This method must return type of columns that must be associated with current widget
    // If you not specify your own type name for column (see 'SetFilterWidgetType' method), GridMvc setup column type name from .Net type ("System.DateTime etc.)
    //
    textFilterWidget.prototype.getAssociatedTypes = function () { return ["System.String", "System.Guid"]; };
    //
    // This method invokes when filter widget was shown on the page
    //
    textFilterWidget.prototype.onShow = function (columnName) {
        var textBox = this.filterData[columnName].container.find(".grid-filter-type");
        if (textBox.length <= 0) return;
        textBox.last().focus();
    };
    //
    // This method specify whether onRender 'Clear filter' button for this widget.
    //
    textFilterWidget.prototype.showClearFilterButton = function () { return true; };
    //
    // This method will invoke when user first clicked on filter button.
    // container - html element, which must contain widget layout;
    // lang - current language settings;
    // typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    // filterType - current filter type (like equals, starts with etc);
    // filterValue - current filter value;
    // cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    //
    textFilterWidget.prototype.onRender = function (container, lang, typeName, columnName, isNullable, values, cb) {
        this.cb = cb;
        this.lang = lang;

        if (!this.filterData) {
            this.filterData = new Object();
        }
        if (!this.filterData[columnName]) {
            this.filterData[columnName] = new Object();
        }
        this.filterData[columnName].container = container;
        this.filterData[columnName].typeName = typeName;
        var cond = values.find(x => x.filterType === 9 && x.filterValue && x.columnName === columnName);
        this.filterData[columnName].condition = cond ? cond.filterValue : "1";
        this.filterData[columnName].values = values.filter(x => x.filterType !== 9 && x.columnName === columnName);
        if (!this.filterData[columnName].values) {
            this.filterData[columnName].values = new Array();
        }
        if (this.filterData[columnName].values.length === 0) {
            this.filterData[columnName].values.push({
                filterType: "2",
                filterValue: "",
                columnName: columnName
            });
        }

        this.renderWidget(columnName);
        this.registerEvents(columnName);
    };
    //
    // Internal method that build widget layout and append it to the widget container
    //
    textFilterWidget.prototype.renderWidget = function (columnName) {
        var html = '<div class="grid-filter-body">';
        for (var i = 0; i < this.filterData[columnName].values.length; i++) {
            if (i === 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-cond form-control">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            else if (i > 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-conddis form-control" disabled="disabled">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            html += '<div class="form-group row">\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html +=     '<label class="control-label">' + this.lang.filterTypeLabel + '</label>';
            }         
            html +=         '<div>\
                                <select class="grid-filter-type form-control">\
                                    <option value="2" ' + (this.filterData[columnName].values[i].filterType.toString() === "2" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.Contains + '</option >\
                                    <option value="1" ' + (this.filterData[columnName].values[i].filterType.toString() === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                                    <option value="10" ' + (this.filterData[columnName].values[i].filterType.toString() === "10" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotEquals + '</option>\
                                    <option value="3" ' + (this.filterData[columnName].values[i].filterType.toString() === "3" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.StartsWith + '</option>\
                                    <option value="4" ' + (this.filterData[columnName].values[i].filterType.toString() === "4" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.EndsWith + '</option>\
                                    <option value="11" ' + (this.filterData[columnName].values[i].filterType.toString() === "11" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.IsNull + '</option>\
                                    <option value="12" ' + (this.filterData[columnName].values[i].filterType.toString() === "12" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.IsNotNull + '</option>\
                                    <option value="13" ' + (this.filterData[columnName].values[i].filterType.toString() === "13" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Duplicated + '</option>\
                                    <option value="14" ' + (this.filterData[columnName].values[i].filterType.toString() === "14" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotDuplicated + '</option>\
                                </select>\
                            </div>\
                        </div>\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html +=     '<label class="control-label">' + this.lang.filterValueLabel + '</label>';
            }   
            html +=         '<div>\
                                <input type="text" class="grid-filter-input form-control" value="' + decodeURIComponent((this.filterData[columnName].values[i].filterValue + '').replace(/\+/g, '%20')) + '" />\
                            </div>\
                        </div>\
                    </div > ';
        }
        html += '<div class="grid-buttons">\
                    <div class="grid-filter-buttons" >\
                        <button type="button" class="btn btn-primary grid-apply" >' + this.lang.applyFilterButtonText + '</button>\
                    </div >\
                    <div class="grid-filter-buttons">\
                        <button type ="button" class="btn btn-primary grid-add"><b>+</b></button>';
        if (this.filterData[columnName].values.length > 1) {
            html +=     '<button type="button" class="btn btn-primary grid-remove"><b>-</b></button>';
        }
        html +=     '</div>\
                </div>\
            </div> ';
        var filterBody = this.filterData[columnName].container.find(".grid-filter-body");
        if (filterBody) {
            filterBody.remove();
        }
        this.filterData[columnName].container.append(html);
    };
    //
    // Internal method that register event handlers for 'apply' button.
    //
    textFilterWidget.prototype.registerEvents = function (columnName) {
        //get apply button from:
        var applyBtn = this.filterData[columnName].container.find(".grid-apply");
        //save current context:
        var self = this;
        var $context = this.filterData[columnName];
        //register onclick event handler
        applyBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                var filters = new Array();
                if (cond) {
                    filters.push({ filterType: "9", filterValue: cond });
                }
                for (var i = 0; i < types.length; i++) {
                    filters.push({ filterType: types[i].value, filterValue: values[i].value });
                }
                self.cb(filters);
            }
        });
        //register onEnter event for filter text box:
        this.filterData[columnName].container.find(".grid-filter-input").keyup(function (event) {
            if (event.keyCode === 13) { applyBtn.click(); }
            if (event.keyCode === 27) { GridMvc.closeOpenedPopups(); }
        });
        //get add button from:
        var addBtn = this.filterData[columnName].container.find(".grid-add");
        //register onclick event handler
        addBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.push({ filterType: "2", filterValue: "", columnName: columnName });
                self.renderWidget(columnName);
                self.registerEvents(columnName);
                self.onShow(columnName);
            }
        });
        //get add button from:
        var removeBtn = this.filterData[columnName].container.find(".grid-remove");
        //register onclick event handler
        removeBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.splice(values.length - 1, 1);
                self.renderWidget(columnName);
                self.registerEvents(columnName);
                self.onShow(columnName);
            }
        });
        var condSelect = this.filterData[columnName].container.find(".grid-filter-cond");
        condSelect.change(function () {
            $(".grid-filter-conddis").val(this.value);
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

    numberFilterWidget.prototype.onShow = function (columnName) {
        var textBox = this.filterData[columnName].container.find(".grid-filter-type");
        if (textBox.length <= 0) return;
        textBox.last().focus();
    };

    numberFilterWidget.prototype.onRender = function (container, lang, typeName, columnName, isNullable, values, cb) {
        this.cb = cb;
        this.lang = lang;

        if (!this.filterData) {
            this.filterData = new Object();
        }
        if (!this.filterData[columnName]) {
            this.filterData[columnName] = new Object();
        }
        this.filterData[columnName].container = container;
        this.filterData[columnName].typeName = typeName;
        var cond = values.find(x => x.filterType === 9 && x.filterValue && x.columnName === columnName);
        this.filterData[columnName].condition = cond ? cond.filterValue : "1";
        this.filterData[columnName].values = values.filter(x => x.filterType !== 9 && x.columnName === columnName);
        if (!this.filterData[columnName].values) {
            this.filterData[columnName].values = new Array();
        }
        if (this.filterData[columnName].values.length === 0) {
            this.filterData[columnName].values.push({
                filterType: "1",
                filterValue: "",
                columnName: columnName
            });
        }

        this.renderWidget(columnName, isNullable);
        this.registerEvents(columnName, isNullable);
    };

    numberFilterWidget.prototype.renderWidget = function (columnName, isNullable) {
        var html = '<div class="grid-filter-body">';
        for (var i = 0; i < this.filterData[columnName].values.length; i++) {
            if (i === 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-cond form-control">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            else if (i > 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-conddis form-control" disabled="disabled">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            html += '<div class="form-group row">\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html += '<label class="control-label">' + this.lang.filterTypeLabel + '</label>';
            }
            html +=         '<div>\
                                <select class="grid-filter-type form-control">\
                                    <option value="1" ' + (this.filterData[columnName].values[i].filterType.toString() === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                                    <option value="10" ' + (this.filterData[columnName].values[i].filterType.toString() === "10" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotEquals + '</option>\
                                    <option value="5" ' + (this.filterData[columnName].values[i].filterType.toString() === "5" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThan + '</option>\
                                    <option value="6" ' + (this.filterData[columnName].values[i].filterType.toString() === "6" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThan + '</option>\
                                    <option value="7" ' + (this.filterData[columnName].values[i].filterType.toString() === "7" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThanOrEquals + '</option>\
                                    <option value="8" ' + (this.filterData[columnName].values[i].filterType.toString() === "8" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThanOrEquals + '</option>';
            if (isNullable.toLowerCase() === "true") {
                html +=             '<option value="11" ' + (this.filterData[columnName].values[i].filterType.toString() === "11" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.IsNull + '</option >\
                                    <option value="12" ' + (this.filterData[columnName].values[i].filterType.toString() === "12" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.IsNotNull + '</option >';
            }
            html +=                 '<option value="13" ' + (this.filterData[columnName].values[i].filterType.toString() === "13" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Duplicated + '</option>\
                                    <option value="14" ' + (this.filterData[columnName].values[i].filterType.toString() === "14" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotDuplicated + '</option>\
                                </select>\
                            </div >\
                        </div>\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html +=     '<label class="control-label">' + this.lang.filterValueLabel + '</label>';
            }
            html +=         '<div>\
                                <input type="text" class="grid-filter-input form-control" value="' + decodeURIComponent((this.filterData[columnName].values[i].filterValue + '').replace(/\+/g, '%20')) + '" />\
                            </div>\
                        </div>\
                    </div> ';
        }
        html += '<div class="grid-buttons">\
                    <div class="grid-filter-buttons" >\
                        <button type="button" class="btn btn-primary grid-apply" >' + this.lang.applyFilterButtonText + '</button>\
                    </div >\
                    <div class="grid-filter-buttons">\
                        <button type ="button" class="btn btn-primary grid-add"><b>+</b></button>';
        if (this.filterData[columnName].values.length > 1) {
            html +=     '<button type="button" class="btn btn-primary grid-remove"><b>-</b></button>';
        }
        html +=     '</div>\
                </div>\
            </div> ';
        var filterBody = this.filterData[columnName].container.find(".grid-filter-body");
        if (filterBody) {
            filterBody.remove();
        }
        this.filterData[columnName].container.append(html);
    };

    numberFilterWidget.prototype.registerEvents = function (columnName, isNullable) {
        var $context = this.filterData[columnName];
        var self = this;
        var applyBtn = this.filterData[columnName].container.find(".grid-apply");
        applyBtn.click(function () {
            var option = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                var filters = new Array();
                if (option) {
                    filters.push({ filterType: "9", filterValue: option });
                }
                for (var i = 0; i < types.length; i++) {
                    filters.push({ filterType: types[i].value, filterValue: values[i].value });
                }
                self.cb(filters);
            }
        });
        var txt = this.filterData[columnName].container.find(".grid-filter-input");
        txt.keyup(function (event) {
            if (event.keyCode === 13) { applyBtn.click(); }
            if (event.keyCode === 27) { GridMvc.closeOpenedPopups(); }
        }).keypress(function (event) { return self.validateInput.call($context, columnName, event); });
        if (this.filterData[columnName].typeName === "System.Byte")
            txt.attr("maxlength", "3");
        //get add button from:
        var addBtn = this.filterData[columnName].container.find(".grid-add");
        //register onclick event handler
        addBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.push({ filterType: "1", filterValue: "", columnName: columnName });
                self.renderWidget(columnName, isNullable);
                self.registerEvents(columnName, isNullable);
                self.onShow(columnName);
            }
        });
        //get add button from:
        var removeBtn = this.filterData[columnName].container.find(".grid-remove");
        //register onclick event handler
        removeBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.splice(values.length - 1, 1);
                self.renderWidget(columnName, isNullable);
                self.registerEvents(columnName, isNullable);
                self.onShow(columnName);
            }
        });
        var condSelect = this.filterData[columnName].container.find(".grid-filter-cond");
        condSelect.change(function () {
            $(".grid-filter-conddis").val(this.value);
        });
    };

    numberFilterWidget.prototype.validateInput = function (columnName, evt) {
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

    dateTimeFilterWidget.prototype.getAssociatedTypes = function () { return ["System.DateTime", "System.Date", "System.DateTimeOffset", "System.DateOnly"]; };

    dateTimeFilterWidget.prototype.showClearFilterButton = function () { return true; };

    dateTimeFilterWidget.prototype.onShow = function (columnName) {
        var textBox = this.filterData[columnName].container.find(".grid-filter-type");
        if (textBox.length <= 0) return;
        textBox.last().focus();
    };

    dateTimeFilterWidget.prototype.onRender = function (container, lang, typeName, columnName, isNullable, values, applycb, data) {
        this.datePickerIncluded = typeof $.fn.datepicker !== 'undefined';
        this.cb = applycb;
        this.data = data;
        this.lang = lang;

        if (!this.filterData) {
            this.filterData = new Object();
        }
        if (!this.filterData[columnName]) {
            this.filterData[columnName] = new Object();
        }
        this.filterData[columnName].container = container;
        this.filterData[columnName].typeName = typeName;
        var cond = values.find(x => x.filterType === 9 && x.filterValue && x.columnName === columnName);
        this.filterData[columnName].condition = cond ? cond.filterValue : "1";
        this.filterData[columnName].values = values.filter(x => x.filterType !== 9 && x.columnName === columnName);
        if (!this.filterData[columnName].values) {
            this.filterData[columnName].values = new Array();
        }
        if (this.filterData[columnName].values.length === 0) {
            this.filterData[columnName].values.push({
                filterType: "1",
                filterValue: "",
                columnName: columnName
            });
        }

        this.renderWidget(columnName, isNullable);
        this.registerEvents(columnName, isNullable);
    };

    dateTimeFilterWidget.prototype.renderWidget = function (columnName, isNullable) {
        this.datePickerIncluded = typeof $.fn.datepicker !== 'undefined';
        var html = '<div class="grid-filter-body">';
        for (var i = 0; i < this.filterData[columnName].values.length; i++) {
            if (i === 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-cond form-control">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            else if (i > 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-conddis form-control" disabled="disabled">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            html += '<div class="form-group row">\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html += '<label class="control-label">' + this.lang.filterTypeLabel + '</label>';
            }
            html +=         '<div>\
                                <select class="grid-filter-type form-control">\
                                    <option value="1" ' + (this.filterData[columnName].values[i].filterType.toString() === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                                    <option value="10" ' + (this.filterData[columnName].values[i].filterType.toString() === "10" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotEquals + '</option>\
                                    <option value="5" ' + (this.filterData[columnName].values[i].filterType.toString() === "5" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThan + '</option>\
                                    <option value="6" ' + (this.filterData[columnName].values[i].filterType.toString() === "6" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThan + '</option>\
                                    <option value="7" ' + (this.filterData[columnName].values[i].filterType.toString() === "7" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThanOrEquals + '</option>\
                                    <option value="8" ' + (this.filterData[columnName].values[i].filterType.toString() === "8" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThanOrEquals + '</option>';
            if (isNullable.toLowerCase() === "true") {
                html +=             '<option value="11" ' + (this.filterData[columnName].values[i].filterType.toString() === "11" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.IsNull + '</option >\
                                    <option value="12" ' + (this.filterData[columnName].values[i].filterType.toString() === "12" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.IsNotNull + '</option >';
            }
            html +=                 '<option value="13" ' + (this.filterData[columnName].values[i].filterType.toString() === "13" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Duplicated + '</option>\
                                    <option value="14" ' + (this.filterData[columnName].values[i].filterType.toString() === "14" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotDuplicated + '</option>\
                                </select>\
                            </div>\
                        </div>\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html +=     '<label class="control-label">' + this.lang.filterValueLabel + '</label>';
            }
            html +=        '<div>\
                                <input id="input-' + columnName + '-' + i.toString() + '" type="text" placeholder="yyyy-mm-dd" class="grid-filter-input form-control" value="' + decodeURIComponent((this.filterData[columnName].values[i].filterValue + '').replace(/\+/g, '%20')) + '" />\
                            </div>';
            html += this.datePickerIncluded ? '<div id="picker-' + columnName + '-' + i.toString() + '" class="grid-filter-datepicker"></div>' : '';
            html +=    '</div>\
                    </div > ';
        }
        html += '<div class="grid-buttons">\
                    <div class="grid-filter-buttons" >\
                        <button type="button" class="btn btn-primary grid-apply" >' + this.lang.applyFilterButtonText + '</button>\
                    </div >\
                    <div class="grid-filter-buttons">\
                        <button type ="button" class="btn btn-primary grid-add"><b>+</b></button>';
        if (this.filterData[columnName].values.length > 1) {
            html +=     '<button type="button" class="btn btn-primary grid-remove"><b>-</b></button>';
        }
        html +=     '</div>\
                </div>\
            </div> ';
        var filterBody = this.filterData[columnName].container.find(".grid-filter-body");
        if (filterBody) {
            filterBody.remove();
        }
        this.filterData[columnName].container.append(html);
    };

    dateTimeFilterWidget.prototype.registerEvents = function (columnName, isNullable) {
        var self = this;
        var $context = this.filterData[columnName];
        var applyBtn = this.filterData[columnName].container.find(".grid-apply");
        applyBtn.click(function () {
            var option = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                var filters = new Array();
                if (option) {
                    filters.push({ filterType: "9", filterValue: option });
                }
                for (var i = 0; i < types.length; i++) {
                    filters.push({ filterType: types[i].value, filterValue: values[i].value });
                }
                self.cb(filters);
            }
        });
        var dateInput = this.filterData[columnName].container.find(".grid-filter-input");
        dateInput.each(function () {
            $(this).click(function () {
                if (self.datePickerIncluded) {
                    var id = $(this).attr("id");
                    var pickerid = id.replace("input-", "picker-");

                    var datePickerOptions = self.data || {};
                    datePickerOptions.format = datePickerOptions.format || "yyyy-mm-dd";
                    datePickerOptions.language = datePickerOptions.language || self.lang.code;

                    var dateContainer = $context.container.find("#" + pickerid);
                    dateContainer.datepicker(datePickerOptions).on('changeDate', function (ev) {                       
                        var inputContainer = $context.container.find("#" + id);
                        inputContainer.val(ev.format());
                        $(this).hide();
                    });
                    if (this.value)
                        dateContainer.datepicker('update', this.value);
                    dateContainer.css({ "position": "relative", "left": "-120px", "top": "0px" });
                    dateContainer.show();
                }
            });
        });
        dateInput.keyup(function (event) {
            if (event.keyCode === 13) { applyBtn.click(); }
            if (event.keyCode === 27) { GridMvc.closeOpenedPopups(); }
        });
        //get add button from:
        var addBtn = this.filterData[columnName].container.find(".grid-add");
        //register onclick event handler
        addBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.push({ filterType: "1", filterValue: "", columnName: columnName });
                self.renderWidget(columnName, isNullable);
                self.registerEvents(columnName, isNullable);
                self.onShow(columnName);
            }
        });
        //get add button from:
        var removeBtn = this.filterData[columnName].container.find(".grid-remove");
        //register onclick event handler
        removeBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.splice(values.length - 1, 1);
                self.renderWidget(columnName, isNullable);
                self.registerEvents(columnName, isNullable);
                self.onShow(columnName);
            }
        });
        var condSelect = this.filterData[columnName].container.find(".grid-filter-cond");
        condSelect.change(function () {
            $(".grid-filter-conddis").val(this.value);
        });
    };

    return dateTimeFilterWidget;
})(window.jQuery);

/***
* TimeFilterWidget - Provides filter interface for time columns (of type "System.TimeOnly").
* If timepicker script included, this widget will render calendar for pick filter values
* In other case he onRender textbox field for specifing time value (more info at http://window.jQueryui.com/)
*/
TimeOnlyFilterWidget = (function ($) {

    function timeOnlyFilterWidget() { }

    timeOnlyFilterWidget.prototype.getAssociatedTypes = function () { return ["System.TimeOnly"]; };

    timeOnlyFilterWidget.prototype.showClearFilterButton = function () { return true; };

    timeOnlyFilterWidget.prototype.onShow = function (columnName) {
        var textBox = this.filterData[columnName].container.find(".grid-filter-type");
        if (textBox.length <= 0) return;
        textBox.last().focus();
    };

    timeOnlyFilterWidget.prototype.onRender = function (container, lang, typeName, columnName, isNullable, values, applycb, data) {
        this.cb = applycb;
        this.data = data;
        this.lang = lang;

        if (!this.filterData) {
            this.filterData = new Object();
        }
        if (!this.filterData[columnName]) {
            this.filterData[columnName] = new Object();
        }
        this.filterData[columnName].container = container;
        this.filterData[columnName].typeName = typeName;
        var cond = values.find(x => x.filterType === 9 && x.filterValue && x.columnName === columnName);
        this.filterData[columnName].condition = cond ? cond.filterValue : "1";
        this.filterData[columnName].values = values.filter(x => x.filterType !== 9 && x.columnName === columnName);
        if (!this.filterData[columnName].values) {
            this.filterData[columnName].values = new Array();
        }
        if (this.filterData[columnName].values.length === 0) {
            this.filterData[columnName].values.push({
                filterType: "1",
                filterValue: "",
                columnName: columnName
            });
        }

        this.renderWidget(columnName, isNullable);
        this.registerEvents(columnName, isNullable);
    };

    timeOnlyFilterWidget.prototype.renderWidget = function (columnName, isNullable) {
        var html = '<div class="grid-filter-body">';
        for (var i = 0; i < this.filterData[columnName].values.length; i++) {
            if (i === 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-cond form-control">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            else if (i > 1) {
                html += '<div class="form-group" style="display:flex;justify-content:center;">\
                            <div>\
                                <select class="grid-filter-conddis form-control" disabled="disabled">\
                                    <option value="1" ' + (this.filterData[columnName].condition === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.And + '</option>\
                                    <option value="2" ' + (this.filterData[columnName].condition === "2" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Or + '</option>\
                                </select>\
                            </div>\
                        </div>';
            }
            html += '<div class="form-group row">\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html += '<label class="control-label">' + this.lang.filterTypeLabel + '</label>';
            }
            html += '<div>\
                                <select class="grid-filter-type form-control">\
                                    <option value="1" ' + (this.filterData[columnName].values[i].filterType.toString() === "1" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Equals + '</option>\
                                    <option value="10" ' + (this.filterData[columnName].values[i].filterType.toString() === "10" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotEquals + '</option>\
                                    <option value="5" ' + (this.filterData[columnName].values[i].filterType.toString() === "5" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThan + '</option>\
                                    <option value="6" ' + (this.filterData[columnName].values[i].filterType.toString() === "6" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThan + '</option>\
                                    <option value="7" ' + (this.filterData[columnName].values[i].filterType.toString() === "7" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.GreaterThanOrEquals + '</option>\
                                    <option value="8" ' + (this.filterData[columnName].values[i].filterType.toString() === "8" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.LessThanOrEquals + '</option>';
            if (isNullable.toLowerCase() === "true") {
                html += '<option value="11" ' + (this.filterData[columnName].values[i].filterType.toString() === "11" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.IsNull + '</option >\
                                    <option value="12" ' + (this.filterData[columnName].values[i].filterType.toString() === "12" ? "selected=\"selected\"" : "") + ' > ' + this.lang.filterSelectTypes.IsNotNull + '</option >';
            }
            html += '<option value="13" ' + (this.filterData[columnName].values[i].filterType.toString() === "13" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.Duplicated + '</option>\
                                    <option value="14" ' + (this.filterData[columnName].values[i].filterType.toString() === "14" ? "selected=\"selected\"" : "") + '>' + this.lang.filterSelectTypes.NotDuplicated + '</option>\
                                </select>\
                            </div>\
                        </div>\
                        <div class="col-md-6 my-2">';
            if (i === 0) {
                html += '<label class="control-label">' + this.lang.filterValueLabel + '</label>';
            }
            html += '<div>\
                                <input id="input-' + columnName + '-' + i.toString() + '" type="time" placeholder="hh:mm" class="grid-filter-input form-control" value="' + decodeURIComponent((this.filterData[columnName].values[i].filterValue + '').replace(/\+/g, '%20')) + '" />\
                            </div>';
            html += '</div>\
                    </div > ';
        }
        html += '<div class="grid-buttons">\
                    <div class="grid-filter-buttons" >\
                        <button type="button" class="btn btn-primary grid-apply" >' + this.lang.applyFilterButtonText + '</button>\
                    </div >\
                    <div class="grid-filter-buttons">\
                        <button type ="button" class="btn btn-primary grid-add"><b>+</b></button>';
        if (this.filterData[columnName].values.length > 1) {
            html += '<button type="button" class="btn btn-primary grid-remove"><b>-</b></button>';
        }
        html += '</div>\
                </div>\
            </div> ';
        var filterBody = this.filterData[columnName].container.find(".grid-filter-body");
        if (filterBody) {
            filterBody.remove();
        }
        this.filterData[columnName].container.append(html);
    };

    timeOnlyFilterWidget.prototype.registerEvents = function (columnName, isNullable) {
        var self = this;
        var $context = this.filterData[columnName];
        var applyBtn = this.filterData[columnName].container.find(".grid-apply");
        applyBtn.click(function () {
            var option = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                var filters = new Array();
                if (option) {
                    filters.push({ filterType: "9", filterValue: option });
                }
                for (var i = 0; i < types.length; i++) {
                    filters.push({ filterType: types[i].value, filterValue: values[i].value });
                }
                self.cb(filters);
            }
        });
        //get add button from:
        var addBtn = this.filterData[columnName].container.find(".grid-add");
        //register onclick event handler
        addBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.push({ filterType: "1", filterValue: "", columnName: columnName });
                self.renderWidget(columnName, isNullable);
                self.registerEvents(columnName, isNullable);
                self.onShow(columnName);
            }
        });
        //get add button from:
        var removeBtn = this.filterData[columnName].container.find(".grid-remove");
        //register onclick event handler
        removeBtn.click(function () {
            var cond = $context.container.find(".grid-filter-cond").val();
            var types = $context.container.find(".grid-filter-type");
            var values = $context.container.find(".grid-filter-input");
            if (types.length === values.length) {
                $context.values = new Array();
                if (cond) {
                    $context.condition = cond;
                }
                for (var i = 0; i < types.length; i++) {
                    $context.values.push({ filterType: types[i].value, filterValue: values[i].value, columnName: columnName });
                }
                $context.values.splice(values.length - 1, 1);
                self.renderWidget(columnName, isNullable);
                self.registerEvents(columnName, isNullable);
                self.onShow(columnName);
            }
        });
        var condSelect = this.filterData[columnName].container.find(".grid-filter-cond");
        condSelect.change(function () {
            $(".grid-filter-conddis").val(this.value);
        });
    };

    return timeOnlyFilterWidget;
})(window.jQuery);

/***
* BooleanFilterWidget - Provides filter interface for boolean columns (of type "System.Boolean").
* Renders two button for filter - true and false
*/
BooleanFilterWidget = (function ($) {

    function booleanFilterWidget() { }

    booleanFilterWidget.prototype.getAssociatedTypes = function () { return ["System.Boolean"]; };

    booleanFilterWidget.prototype.showClearFilterButton = function () { return true; };

    booleanFilterWidget.prototype.onRender = function (container, lang, typeName, columnName, isNullable, values, cb) {
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
                        <li><a class="grid-filter-choose ' + (this.value.filterValue === "true" ? "choose-selected" : "") + '" data-value="true" href="javascript:void(0);">' + this.lang.boolTrueLabel + '</a></li>\
                        <li><a class="grid-filter-choose ' + (this.value.filterValue === "false" ? "choose-selected" : "") + '" data-value="false" href="javascript:void(0);">' + this.lang.boolFalseLabel + '</a></li>\
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

/***
* ListFilterWidget - Provides filter interface for list filter columns .
* Renders two button for filter - true and false
*/
ListFilterWidget = (function ($) {

    function listFilterWidget() { }

    listFilterWidget.prototype.getAssociatedTypes = function () { return ["ListFilter"]; };

    listFilterWidget.prototype.showClearFilterButton = function () { return true; };

    listFilterWidget.prototype.onRender = function (container, lang, typeName, columnName, isNullable, values, cb, data) {
        this.cb = cb;
        this.data = data;
        this.lang = lang;

        if (!this.filterData) {
            this.filterData = new Object();
        }
        if (!this.filterData[columnName]) {
            this.filterData[columnName] = new Object();
        }
        this.filterData[columnName].container = container;
        this.filterData[columnName].typeName = typeName;
        // conditions is always "OR"
        this.filterData[columnName].condition = "2";
        this.filterData[columnName].values = values.filter(x => x.filterType !== 9 && x.columnName === columnName);
        if (!this.filterData[columnName].values) {
            this.filterData[columnName].values = new Array();
        }
        if (this.filterData[columnName].values.length === 0) {
            this.filterData[columnName].values.push({
                filterType: "1",
                filterValue: "",
                columnName: columnName
            });
        }

        this.renderWidget(columnName);
        this.registerEvents(columnName);
    };

    listFilterWidget.prototype.renderWidget = function (columnName) {
        var html = '<div class="grid-filter-body">';
        html +=        '<label>' + this.lang.filterValueLabel + '</label>\
                        <ul class="menu-list">';
        for (var i = 0; i < this.data.length; i++) {
            var checked = this.filterData[columnName].values.some(x => x.filterType === 1 && x.filterValue === this.data[i].Value && x.columnName === columnName);
            html += '<li><input type="checkbox" class="grid-filter-list" '
                + (checked ? "checked" : "")
                + ' value="' + this.data[i].Value + '" /> ' + this.data[i].Title + '</li >';
        }
        html +=        '</ul>';
        html +=        '<div class="grid-buttons">\
                            <div class="grid-filter-buttons" >\
                                <button type="button" class="btn btn-primary grid-apply" >' + this.lang.applyFilterButtonText + '</button>\
                            </div>\
                        </div>\
                    </div> ';
        this.filterData[columnName].container.append(html);
    };

    listFilterWidget.prototype.registerEvents = function (columnName) {
        var $context = this.filterData[columnName];
        var self = this;
        var applyBtn = this.filterData[columnName].container.find(".grid-apply");
        applyBtn.click(function () {
            var values = $context.container.find(".grid-filter-list");
            var filters = new Array();
            for (var i = 0; i < values.length; i++) {
                if (values[i].checked) {
                    filters.push({ filterType: "1", filterValue: values[i].value });
                }             
            }
            if (filters.length > 1 ) {
                filters.push({ filterType: "9", filterValue: "2" });
            }
            self.cb(filters);
        });
    };

    return listFilterWidget;
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