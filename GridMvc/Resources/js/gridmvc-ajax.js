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
                filters.push({ filterValue: this.urldecode(opt[i].FilterValue), filterType: opt[i].FilterType, columnName: opt[i].ColumnName });
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
                self.currentPage = "1";
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
        changePageSize: function(pageSize) {
            var self = this;
            self.pageSize = this.getPageSizeQueryData(pageSize);
            self.loadPage();
        },
        gotoPage: function (page) {
            var self = this;
            self.currentPage = page;
            self.loadPage();
        },
        removeAllFilters: function () {
            var self = this;
            self.gridColumnFilters = null;
            self.clearInitialFilters = new Array();
            self.loadPage();
        },
        parseExtSortValues: function (extSortData) {
            var opt = $.parseJSON(extSortData);
            return { columnName: this.urldecode(opt.ColumnName), direction: opt.Direction, id: opt.Id };
        },
        // operation values: 0 - remove, 1 - add, 2 - change
        applyExtSortValues: function (columnName, direction, operation) {
            var self = this;
            self.gridExtSort = null;
            var extSorts = self.jqContainer.find(".grid-extsort-column");

            self.gridExtSort = "";

            var extSortData;
            if (this.options.extsortable) { // ext sorting enabled
                for (var i = 0; i < extSorts.length; i++) {
                    if ($(extSorts[i]).attr("data-name") === columnName && operation === 0) {
                        continue;
                    }
                    else if ($(extSorts[i]).attr("data-name") === columnName && operation === 2) {
                        extSortData = this.parseExtSortValues($(extSorts[i]).attr("data-extsortdata"));
                        if (self.gridExtSort.length > 0) self.gridExtSort += "&";
                        self.gridExtSort += this.getExtSortQueryData(columnName, direction, extSortData.id);
                    }
                    else {
                        extSortData = this.parseExtSortValues($(extSorts[i]).attr("data-extsortdata"));
                        if (self.gridExtSort.length > 0) self.gridExtSort += "&";
                        self.gridExtSort += this.getExtSortQueryData(extSortData.columnName, extSortData.direction, extSortData.id);
                    }     
                }
            }

            if (operation === 1) {
                var extSortStr = "grid-sorting=" + columnName;
                var regex = new RegExp(extSortStr, 'g');
                var id = self.gridExtSort.match(regex);
                if (!id) {
                    id = self.gridExtSort.match(/grid-sorting=/g);
                    if (self.gridExtSort.length > 0) self.gridExtSort += "&";
                    if (id)
                        id = id.length + 1;
                    else
                        id = 1;
                    self.gridExtSort += this.getExtSortQueryData(columnName, direction, id);
                }
            }

            if (self.gridExtSort.indexOf("?") === -1) {
                self.gridExtSort = "?" + self.gridExtSort;
            }

            self.loadPage();
        },
        ajaxify: function (options) {
            var self = this;
            if (this.options.currentPage) {
                self.currentPage = this.options.currentPage;
            }
            else {
                self.currentPage = "1";
            }
            self.pagedDataAction = options.getPagedData;
            self.subGridDataAction = options.getSubGridData;
            self.data = options.data;
            self.token = options.token;
            self.gridSort = self.jqContainer.find("div.sorted a").attr('href');
            self.gridColumnFilters = "";
            self.gridSearch = "";
            self.pageSize = "";
            self.gridExtSort = "";
            var $namedGrid = $('[data-gridname="' + self.jqContainer.data("gridname") + '"]');
            self.jqContainer = $namedGrid.length === 1 ? $namedGrid : self.jqContainer;

            if (self.gridSort) {
                if (self.gridSort.indexOf("grid-dir=0") !== -1) {
                    self.gridSort = self.gridSort.replace("grid-dir=0", "grid-dir=1");
                } else {
                    self.gridSort = self.gridSort.replace("grid-dir=1", "grid-dir=0");
                }
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

            self.uint6ToB64 = function (nUint6) {

                return nUint6 < 26 ?
                    nUint6 + 65
                    : nUint6 < 52 ?
                        nUint6 + 71
                        : nUint6 < 62 ?
                            nUint6 - 4
                            : nUint6 === 62 ?
                                43
                                : nUint6 === 63 ?
                                    47
                                    :
                                    65;

            };

            self.base64EncArr = function (aBytes) {

                var eqLen = (3 - (aBytes.length % 3)) % 3, sB64Enc = "";

                for (var nMod3, nLen = aBytes.length, nUint24 = 0, nIdx = 0; nIdx < nLen; nIdx++) {
                    nMod3 = nIdx % 3;
                    /* Uncomment the following line in order to split the output in lines 76-character long: */
                    /*
                    if (nIdx > 0 && (nIdx * 4 / 3) % 76 === 0) { sB64Enc += "\r\n"; }
                    */
                    nUint24 |= aBytes[nIdx] << (16 >>> nMod3 & 24);
                    if (nMod3 === 2 || aBytes.length - nIdx === 1) {
                        sB64Enc += String.fromCharCode(self.uint6ToB64(nUint24 >>> 18 & 63), self.uint6ToB64(nUint24 >>> 12 & 63), self.uint6ToB64(nUint24 >>> 6 & 63), self.uint6ToB64(nUint24 & 63));
                        nUint24 = 0;
                    }
                }

                return eqLen === 0 ?
                    sB64Enc
                    :
                    sB64Enc.substring(0, sB64Enc.length - eqLen) + (eqLen === 1 ? "=" : "==");

            };

            self.strToUTF8Arr = function (sDOMStr) {

                var aBytes, nChr, nStrLen = sDOMStr.length, nArrLen = 0;

                /* mapping... */

                for (var nMapIdx = 0; nMapIdx < nStrLen; nMapIdx++) {
                    nChr = sDOMStr.charCodeAt(nMapIdx);
                    nArrLen += nChr < 0x80 ? 1 : nChr < 0x800 ? 2 : nChr < 0x10000 ? 3 : nChr < 0x200000 ? 4 : nChr < 0x4000000 ? 5 : 6;
                }

                aBytes = new Uint8Array(nArrLen);

                /* transcription... */

                for (var nIdx = 0, nChrIdx = 0; nIdx < nArrLen; nChrIdx++) {
                    nChr = sDOMStr.charCodeAt(nChrIdx);
                    if (nChr < 128) {
                        /* one byte */
                        aBytes[nIdx++] = nChr;
                    } else if (nChr < 0x800) {
                        /* two bytes */
                        aBytes[nIdx++] = 192 + (nChr >>> 6);
                        aBytes[nIdx++] = 128 + (nChr & 63);
                    } else if (nChr < 0x10000) {
                        /* three bytes */
                        aBytes[nIdx++] = 224 + (nChr >>> 12);
                        aBytes[nIdx++] = 128 + (nChr >>> 6 & 63);
                        aBytes[nIdx++] = 128 + (nChr & 63);
                    } else if (nChr < 0x200000) {
                        /* four bytes */
                        aBytes[nIdx++] = 240 + (nChr >>> 18);
                        aBytes[nIdx++] = 128 + (nChr >>> 12 & 63);
                        aBytes[nIdx++] = 128 + (nChr >>> 6 & 63);
                        aBytes[nIdx++] = 128 + (nChr & 63);
                    } else if (nChr < 0x4000000) {
                        /* five bytes */
                        aBytes[nIdx++] = 248 + (nChr >>> 24);
                        aBytes[nIdx++] = 128 + (nChr >>> 18 & 63);
                        aBytes[nIdx++] = 128 + (nChr >>> 12 & 63);
                        aBytes[nIdx++] = 128 + (nChr >>> 6 & 63);
                        aBytes[nIdx++] = 128 + (nChr & 63);
                    } else /* if (nChr <= 0x7fffffff) */ {
                        /* six bytes */
                        aBytes[nIdx++] = 252 + (nChr >>> 30);
                        aBytes[nIdx++] = 128 + (nChr >>> 24 & 63);
                        aBytes[nIdx++] = 128 + (nChr >>> 18 & 63);
                        aBytes[nIdx++] = 128 + (nChr >>> 12 & 63);
                        aBytes[nIdx++] = 128 + (nChr >>> 6 & 63);
                        aBytes[nIdx++] = 128 + (nChr & 63);
                    }
                }
                return aBytes;
            };

            self.getState = function () {
                var data = new Object();

                var myColFilters = URI.parseQuery(self.gridColumnFilters)["grid-filter"];
                if (Array.isArray(myColFilters)) {
                    data['grid-filter'] = myColFilters.join("|");
                }
                else {
                    data['grid-filter'] = myColFilters;
                }

                var myExtSort = URI.parseQuery(self.gridExtSort)["grid-sorting"];
                if (Array.isArray(myExtSort)) {
                    data['grid-sorting'] = myExtSort.join("|");
                }
                else {
                    data['grid-sorting'] = myExtSort;
                }

                var mySearch = URI.parseQuery(self.gridSearch)["grid-search"];
                if (Array.isArray(mySearch)) {
                    data['grid-search'] = mySearch.join("|");
                }
                else {
                    data['grid-search'] = mySearch;
                }

                var myPageSize = URI.parseQuery(self.pageSize)["grid-pagesize"];
                if (Array.isArray(myPageSize)) {
                    data['grid-pagesize'] = myPageSize.join("|");
                }
                else {
                    data['grid-pagesize'] = myPageSize;
                }

                if (self.gridSort) {
                    var mySort = URI.parseQuery(self.gridSort);
                    data["grid-column"] = mySort["grid-column"];
                    data["grid-dir"] = mySort["grid-dir"];
                }

                if (self.clearInitialFilters) {
                    data["grid-clearinitfilter"] = self.clearInitialFilters.join("|");
                }

                data["grid-page"] = self.currentPage;

                var input = JSON.stringify(data);
                var UTF8Input = self.strToUTF8Arr(input);
                var base64str = self.base64EncArr(UTF8Input);
                base64str = base64str.replace('+', '.');
                base64str = base64str.replace('/', '_');
                base64str = base64str.replace('=', '-');
                return base64str;
            };

            self.getGridUrl = function (griLoaddAction, filters, extSort, search, pageSize) {
                var gridQuery = URI(griLoaddAction);

                var mySearch = URI.parseQuery(search);
                if (mySearch['grid-search']) {
                    gridQuery.addSearch("grid-search", mySearch["grid-search"]);
                }

                var myPageSize = URI.parseQuery(pageSize);
                if (myPageSize['grid-pagesize']) {
                    gridQuery.addSearch("grid-pagesize", myPageSize["grid-pagesize"]);
                }

                var myColFilters = URI.parseQuery(filters);
                if (myColFilters['grid-filter']) {
                    gridQuery.addSearch("grid-filter", myColFilters["grid-filter"]);
                }

                var myColExtSort = URI.parseQuery(extSort);
                if (myColExtSort['grid-sorting']) {
                    gridQuery.addSearch("grid-sorting", myColExtSort["grid-sorting"]);
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

                        var search = $(this).attr('href');
                        var searchMatch = search.match(/grid-column=\w+/);
                        if (searchMatch)
                            self.gridSort = search.substr(searchMatch.index);

                        // load new data
                        self.loadPage();
                    });
                });

                self.jqContainer.find(".grid-filter").each(function () {
                    $(this).click(function (e) {
                        // ListFilterWidget imput click not working when adding preventDefault
                        //e.preventDefault();
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

                self.jqContainer.find(".grid-search-input").each(function () {
                    $(this).keyup(function (e) {
                        if (e.keyCode === 13) {
                            e.preventDefault();
                            var searchText = $(this).val();
                            self.applySearchValues(searchText, false);
                        }
                    });
                });

                self.jqContainer.find(".grid-search-clear").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        return self.applySearchValues("", true);
                    });
                });

                self.jqContainer.find(".grid-change-page-size-input").each(function () {
                    $(this).keydown(function (e) {
                        if (e.keyCode === 9 || e.keyCode === 13) {
                            e.preventDefault();
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

                self.jqContainer.find(".grid-goto-page-input").each(function () {
                    $(this).keydown(function (e) {
                        if (e.keyCode === 9 || e.keyCode === 13) {
                            e.preventDefault();
                            var page = $(this).val();
                            var x = parseInt(page, 10);
                            if (x > 0) {
                                self.gotoPage(page);
                            }
                            else {
                                $(this).val(this.defaultValue);
                            }
                        }
                    });
                });

                self.jqContainer.find(".grid-button-all-filters-clear").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        self.removeAllFilters();
                    });
                });

                self.jqContainer.find(".grid-extsort-draggable").each(function () {
                    $(this).on('dragstart', function (e) {
                        e.originalEvent.dataTransfer.setData("text", e.target.text);
                        e.originalEvent.dataTransfer.setData("column", e.target.dataset.column);
                        if (e.target.dataset.sorted)
                            e.originalEvent.dataTransfer.setData("sorted", e.target.dataset.sorted);
                    });
                });

                self.jqContainer.find(".grid-extsort-droppable").each(function () {
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
                        var column = e.originalEvent.dataTransfer.getData("column");
                        var sorted = e.originalEvent.dataTransfer.getData("sorted") === "desc" ? 1 : 0;
                        if (column && column !== "undefined")
                            self.applyExtSortValues(column, sorted, 1);
                    });
                }); 

                self.jqContainer.find(".grid-extsort-column > span.sorted-asc > a").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        var column = $(this).attr('data-column');
                        if (column)
                            self.applyExtSortValues(column, 1, 2);
                    });
                });

                self.jqContainer.find(".grid-extsort-column > span.sorted-desc > a").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        var column = $(this).attr('data-column');
                        if (column)
                            self.applyExtSortValues(column, 0, 2);
                    });
                });

                self.jqContainer.find(".grid-extsort-column > a").each(function () {
                    $(this).click(function (e) {
                        e.preventDefault();
                        var column = $(this).attr('data-column');
                        if (column)
                            self.applyExtSortValues(column, 0, 0);
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
                var gridUrl = self.getGridUrl(self.pagedDataAction, self.gridColumnFilters, self.gridExtSort, self.gridSearch, self.pageSize);

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
                    self.initChangePageSize();
                    self.initExtSort();
                    self.initGroup();             
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

            self.initChangePageSize = function () {
                self.pageSize = null;
                var size = self.jqContainer.find(".grid-change-page-size-input").first().val();
                self.pageSize = this.getPageSizeQueryData(size);
            };

            self.initExtSort = function () {
                self.gridExtSort = null;
                var extSorts = self.jqContainer.find(".grid-extsort-column");

                self.gridExtSort = "";

                if (this.options.extsortable) { // extended sorting enabled
                    for (var i = 0; i < extSorts.length; i++) {
                        var extSortData = this.parseExtSortValues($(extSorts[i]).attr("data-extsortdata"));
                        if (self.gridExtSort.length > 0) self.gridExtSort += "&";
                        self.gridExtSort += this.getExtSortQueryData(extSortData.columnName, extSortData.direction, extSortData.id);
                    }
                }

                if (self.gridExtSort.indexOf("?") === -1) {
                    self.gridExtSort = "?" + self.gridExtSort;
                }
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
            self.initChangePageSize();;
            self.initExtSort();      
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