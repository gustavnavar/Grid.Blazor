(function ($) {
    var plugin = GridMvc.prototype;

    // store copies of the original plugin functions before overwriting
    var functions = {};
    for (var i in plugin) {
        if (typeof plugin[i] === 'function') {
            functions[i] = plugin[i];
        }
    }

    // extend existing functionality of the gridmvc plugin
    $.extend(true, plugin, {
        parseFilterValues: function (filterData) {
            var opt = $.parseJSON(filterData);
            var filters = [];
            for (var i = 0; i < opt.length; i++) {
                filters.push({ filterValue: opt[i].FilterValue, filterType: opt[i].FilterType, columnName: opt[i].ColumnName });
            }
            return filters;
        },
        applyFilterValues: function (initialUrl, columnName, values, skip) {
            var self = this;
            self.gridColumnFilters = null;
            var filters = self.jqContainer.find(".grid-filter");
            var url = URI(initialUrl).normalizeSearch().search();

            if (url.length > 0)
                url += "&";

            self.gridColumnFilters = "";
            if (!skip) {
                self.gridColumnFilters += this.getFilterQueryData(columnName, values);
            }

            if (this.options.multiplefilters) { //multiple filters enabled
                for (var i = 0; i < filters.length; i++) {
                    if ($(filters[i]).attr("data-name") !== columnName) {
                        var filterData = this.parseFilterValues($(filters[i]).attr("data-filterdata"));
                        if (filterData.length === 0) continue;
                        if (self.gridColumnFilters.length > 0) self.gridColumnFilters += "&";
                        self.gridColumnFilters += this.getFilterQueryData($(filters[i]).attr("data-name"), filterData);
                    } else {
                        continue;
                    }
                }
                if (self.initialFilters.includes(columnName) && !self.clearInitialFilters.includes(columnName)) {
                    self.clearInitialFilters.push(columnName);
                }
            }
            else {
                for (var j = 0; j < self.initialFilters.length; j++) {
                    if (!self.clearInitialFilters.includes(self.initialFilters[j])) {
                        self.clearInitialFilters.push(self.initialFilters[j]);
                    }
                }
            }

            if (self.gridColumnFilters.length > 0) {
                url += "&" + self.gridColumnFilters;
            }
            var fullSearch = url;
            if (fullSearch.indexOf("?") === -1) {
                fullSearch = "?" + fullSearch;
            }

            self.gridColumnFilters = fullSearch;
            if (this.options.currentPage) {
                self.currentPage = this.options.currentPage;
            }
            else {
                self.currentPage = 1;
            }

            self.loadPage();
        },
        applySearchValues: function (searchText, skip) {  
            var self = this;
            if (skip) {
                self.gridSearch = "";
            }
            else {
                self.gridSearch = this.getSearchQueryData(searchText);
            }

            self.loadPage();
        },
        ajaxify: function (options) {
            var self = this;
            if (this.options.currentPage) {
                self.currentPage = this.options.currentPage;
            }
            else {
                self.currentPage = 1;
            }
            self.pagedDataAction = options.getPagedData;
            self.subGridDataAction = options.getSubGridData;
            self.data = options.data;
            self.token = options.token;
            self.gridSort = self.jqContainer.find("div.sorted a").attr('href');
            self.gridColumnFilters = "";
            self.gridSearch = "";
            var $namedGrid = $('[data-gridname="' + self.jqContainer.data("gridname") + '"]');
            self.jqContainer = $namedGrid.length === 1 ? $namedGrid : self.jqContainer;

            if (self.gridSort) {
                if (self.gridSort.indexOf("grid-dir=0") !== -1) {
                    self.gridSort = self.gridSort.replace("grid-dir=0", "grid-dir=1");
                } else {
                    self.gridSort = self.gridSort.replace("grid-dir=1", "grid-dir=0");
                }
                self.orginalSort = self.gridSort;
            }

            var initialFiltersString = self.jqContainer.attr("data-initfilters");
            self.initialFilters = null;
            if (initialFiltersString) {
                self.initialFilters = initialFiltersString.split(',');
            }
            else {
                self.initialFilters = new Array();
            }
            var clearInitialFiltersString = self.jqContainer.find(".grid-filter").attr("data-clearinitfilter");
            self.clearInitialFilters = null;
            if (clearInitialFiltersString) {
                self.clearInitialFilters = clearInitialFiltersString.split(',');
            }
            else {
                self.clearInitialFilters = new Array();
            }

            self.getGridParameters = function () {
                return self.getGridUrl("", self.gridColumnFilters, self.gridSearch);
            };

            self.getGridUrl = function (griLoaddAction, filters, search) {
                var gridQuery = URI(griLoaddAction);

                var mySearch = URI.parseQuery(search);
                if (mySearch['grid-search']) {
                    gridQuery.addSearch("grid-search", mySearch["grid-search"]);
                }

                var myColFilters = URI.parseQuery(filters);
                if (myColFilters['grid-filter']) {
                    gridQuery.addSearch("grid-filter", myColFilters["grid-filter"]);
                }

                if (self.gridSort) {
                    var mySort = URI.parseQuery(self.gridSort);
                    gridQuery.addSearch("grid-column", mySort["grid-column"]);
                    gridQuery.addSearch("grid-dir", mySort["grid-dir"]);
                }

                if (self.clearInitialFilters) {
                    for (var i = 0; i < self.clearInitialFilters.length; i++) {
                        gridQuery.addSearch("grid-clearinitfilter", self.clearInitialFilters[i]);
                    }
                }

                gridQuery.removeQuery("grid-page");
                gridQuery.addQuery("grid-page", self.currentPage);
                var gridUrl = URI.decode(gridQuery);
                return gridUrl;
            };

            self.getSubGridUrl = function (griLoaddAction, keys, values) {
                if (keys && values && keys.length === values.length) {
                    var gridQuery = URI(griLoaddAction);

                    for (var i = 0; i < values.length; i++) {
                        gridQuery.addSearch(keys[i], values[i]);
                    }

                    var gridUrl = URI.decode(gridQuery);
                    return gridUrl;
                }
            };

            self.setupGridHeaderEvents = function () {
                self.jqContainer.find(".grid-header-title > a").each(function () {
                    $(this).click(function (e) {
                        self.gridSort = '';
                        e.preventDefault();

                        // remove grid sort arrows
                        self.jqContainer.find(".grid-header-title").removeClass("sorted-asc");
                        self.jqContainer.find(".grid-header-title").removeClass("sorted-desc");

                        var search = $(this).attr('href');
                        var isAscending = search.indexOf("grid-dir=1") !== -1;
                        self.gridSort = search.substr(search.match(/grid-column=\w+/).index);

                        // load new data
                        self.loadPage();

                        // update link to sort in opposite direction
                        if (isAscending) {
                            $(this).attr('href', search.replace("grid-dir=1", "grid-dir=0"));
                        } else {
                            $(this).attr('href', search.replace("grid-dir=0", "grid-dir=1"));
                        }

                        // add new grid sort arrow
                        var newSortClass = isAscending ? "sorted-desc" : "sorted-asc";
                        $(this).parent(".grid-header-title").addClass(newSortClass);
                        $(this).parent(".grid-header-title").children("span").remove();
                        $(this).parent(".grid-header-title").append($("<span/>").addClass("grid-sort-arrow"));
                    });
                });            

                self.jqContainer.find(".grid-filter").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        return self.openFilterPopup.call(this, self, self.filterMenuHtml());
                    });
                });

                self.jqContainer.find(".grid-search-apply").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        var searchText = self.jqContainer.find(".grid-search-input").first().val();
                        return self.applySearchValues(searchText, false);
                    });
                });

                self.jqContainer.find(".grid-search-clear").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        return self.applySearchValues("", true);
                    });
                });
            };

            self.setupPagerLinkEvents = function () {
                self.jqContainer.find(".grid-page-link").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        var pageNumber = $(this).attr('data-page');
                        self.currentPage = pageNumber;
                        self.loadPage();
                    });
                });
            };

            self.loadPage = function () {
                var dfd = new $.Deferred();
                var gridUrl = self.getGridUrl(self.pagedDataAction, self.gridColumnFilters, self.gridSearch);

                $.ajax({
                    url: gridUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: self.data,
                    headers: {
                        "RequestVerificationToken": self.token
                    }
                })
                .done(function (response) {
                    self.jqContainer.hide();
                    self.jqContainer.html(response);
                    self.jqContainer.html(self.jqContainer.children().first().html());

                    // subgrids and others must be initialized after each page load
                    self.setupGridHeaderEvents();
                    self.setupPagerLinkEvents();
                    self.initFilters();
                    self.initSearch();
                    self.initSubGrids();

                    self.jqContainer.show();
                    self.notifyOnGridLoaded(response, $.Event("GridLoaded"));
                })
                .fail(function () {
                    self.notifyOnGridError(null, $.Event("GridError"));
                })
                .always(function (response) {
                    dfd.resolve(response);
                });

                return dfd.promise();
            };

            self.loadSubGridPage = function (values, subGridRow, td, cols) {
                var dfd = new $.Deferred();
                var gridUrl = self.getSubGridUrl(self.subGridDataAction, self.keys, values);

                $.ajax({
                    url: gridUrl,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    headers: {
                        "RequestVerificationToken": self.token
                    }
                })
                .done(function (response) {
                    subGridRow.html("<td></td><td colspan=" + cols + ">" + response + "</td>");

                    // init grid javascript
                    var newGrid = subGridRow.find(".grid-mvc");
                    newGrid.gridmvc();
                    window.pageGrids[newGrid.attr("data-gridname")].ajaxify({
                        getPagedData: gridUrl,
                        token: self.token
                    });

                    // change is-rendered attrib
                    td.attr("data-is-rendered", "true");
                })
                .always(function (response) {
                    dfd.resolve(response);
                });

                return dfd.promise();
            };

            self.initFilters = function () {
                self.gridColumnFilters = null;
                var filters = self.jqContainer.find(".grid-filter");
                var url = URI("").normalizeSearch().search();

                self.gridColumnFilters = "";

                if (this.options.multiplefilters) { //multiple filters enabled
                    for (var i = 0; i < filters.length; i++) {
                        var filterData = this.parseFilterValues($(filters[i]).attr("data-filterdata"));
                        if (filterData.length === 0) continue;
                        if (self.gridColumnFilters.length > 0) self.gridColumnFilters += "&";
                        self.gridColumnFilters += this.getFilterQueryData($(filters[i]).attr("data-name"), filterData);
                    }
                }

                if (self.gridColumnFilters.length > 0) {
                    url += "&" + self.gridColumnFilters;
                }
                var fullSearch = url;
                if (fullSearch.indexOf("?") === -1) {
                    fullSearch = "?" + fullSearch;
                }

                self.gridColumnFilters = fullSearch;
            };

            self.initSearch = function () {
                self.gridSearch = null;
                var search = self.jqContainer.find(".grid-search-input").first().val();
                self.gridSearch = this.getSearchQueryData(search);
            };

            self.initSubGrids = function () {
                self.keys = new Array();
                var keysString = this.jqContainer.attr("data-keys");
                if (keysString) {
                    self.keys = keysString.split(',');
                }

                this.jqContainer.find(".grid-subgrid").each(function () {
                    $(this).click(function (e) {
                        e.stopPropagation();
                        var cols = $(this).parent().children().length;
                        var subGridRow = $(this).parent().next();
                        if (subGridRow && subGridRow.hasClass("subgrid-row")) {
                            var isVisible = $(this).attr("data-is-visible");
                            if (isVisible && isVisible === "true") {
                                // hide subgrid row
                                subGridRow.hide();
                                // change is-visible attrib and caret
                                $(this).attr("data-is-visible", "false");
                                var spanCaretDown = $(this).find(".subgrid-caret-down");
                                spanCaretDown.removeClass("subgrid-caret-down");
                                spanCaretDown.addClass("subgrid-caret");
                            }
                            else {
                                var isRendered = $(this).attr("data-is-rendered");
                                if (isRendered && isRendered === "false") {
                                    // get query values
                                    var values = new Array();
                                    var valuesString = $(this).attr("data-values");
                                    if (valuesString) {
                                        values = valuesString.split(',');
                                    }
                                    if (self.keys && self.keys.length === values.length) {
                                        self.loadSubGridPage(values, subGridRow, $(this), cols - 1);
                                    }
                                }
                                // show subgrid row
                                $(this).parent().next().show();
                                // change is-visible attrib and caret
                                $(this).attr("data-is-visible", "true");
                                var spanCaret = $(this).find(".subgrid-caret");
                                spanCaret.removeClass("subgrid-caret");
                                spanCaret.addClass("subgrid-caret-down");
                            }
                        }
                    });
                });
            };

            self.setupGridHeaderEvents();
            self.setupPagerLinkEvents();
            self.initFilters();
            self.initSearch();
            self.initSubGrids();
        },
        onGridLoaded: function (func) {
            this.events.push({ name: "onGridLoaded", callback: func });
        },
        notifyOnGridLoaded: function (data, e) {
            e.data = data;
            this.notifyEvent("onGridLoaded", e);
        },
        onGridError: function (func) {
            this.events.push({ name: "onGridError", callback: func });
        },
        notifyOnGridError: function (data, e) {
            e.data = data;
            this.notifyEvent("onGridError", e);
        }
    });
})(jQuery);