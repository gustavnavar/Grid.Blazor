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

            if (!self.clearInitFilters.includes(columnName)) {
                self.clearInitFilters.push(columnName);
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

            var clearInitFiltersString = self.jqContainer.find(".grid-filter").attr("data-clearinitfilter");
            self.clearInitFilters = null;
            if (clearInitFiltersString) {
                self.clearInitFilters = clearInitFiltersString.split(',');
            }
            else {
                self.clearInitFilters = new Array();
            }

            self.getGridParameters = function () {
                return self.getGridUrl("", self.gridColumnFilters, self.gridSearch);
            };

            self.getGridUrl = function (griLoaddAction, filters, search) {
                var gridQuery = "?";
                gridQuery = URI(gridQuery);

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

                if (self.clearInitFilters) {
                    for (var i = 0; i < self.clearInitFilters.length; i++) {
                        gridQuery.addSearch("grid-clearinitfilter", self.clearInitFilters[i]);
                    }
                }

                var gridUrl = URI(griLoaddAction).addQuery(gridQuery.search().replace("?", ""));
                gridUrl = URI(gridUrl).removeQuery("grid-page");
                gridUrl = gridUrl.addQuery("grid-page", self.currentPage);
                gridUrl = URI.decode(gridUrl);
                return gridUrl;
            };

            self.setupGridHeaderEvents = function () {
                self.jqContainer.on('click', '.grid-header-title > a', function (e) {
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
                self.jqContainer.on('click', '.grid-filter', function (e) {
                    e.preventDefault();
                    return self.openFilterPopup.call(this, self, self.filterMenuHtml());
                });
                self.jqContainer.on('click', '.grid-search-apply', function (e) {
                    e.preventDefault();
                    var searchText = self.jqContainer.find(".grid-search-input").first().val();
                    return self.applySearchValues(searchText, false);
                });
                self.jqContainer.on('click', '.grid-search-clear', function (e) {
                    e.preventDefault();
                    return self.applySearchValues("", true);
                });
            };

            self.setupPagerLinkEvents = function () {
                self.jqContainer.on("click", ".grid-page-link", function (e) {
                    e.preventDefault();
                    var pageNumber = $(this).attr('data-page');
                    self.currentPage = pageNumber;
                    self.loadPage();
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

            self.setupGridHeaderEvents();
            self.setupPagerLinkEvents();
            self.initFilters();
            self.initSearch();
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