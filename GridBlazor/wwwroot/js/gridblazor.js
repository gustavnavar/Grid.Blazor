window.gridJsFunctions = {
    focusElement: function (element) {
        if(element)
            element.focus();
    },
    isDateTimeLocalSupported: function () {
        var elem = document.createElement("input");
        elem.setAttribute("type", "datetime-local");
        return elem.type !== "text";
    },
    isWeekSupported: function () {
        var elem = document.createElement("input");
        elem.setAttribute("type", "week");
        return elem.type !== "text";
    },
    isMonthSupported: function () {
        var elem = document.createElement("input");
        elem.setAttribute("type", "month");
        return elem.type !== "text";
    },
    // A popup measures itself when it opens, and a window that changes size afterwards leaves
    // it measured against a viewport that no longer exists - a phone rotated with the picker
    // open is the case that bites. One listener per open popup, removed when it closes, and
    // debounced because a drag fires resize dozens of times a second and each one costs a
    // round trip into .NET plus a re-measure.
    resizeHandlers: {},
    addResizeHandler: function (id, dotNetRef) {
        window.gridJsFunctions.removeResizeHandler(id);
        var timer = null;
        var handler = function () {
            if (timer) clearTimeout(timer);
            timer = setTimeout(function () { dotNetRef.invokeMethodAsync('OnWindowResized'); }, 150);
        };
        window.gridJsFunctions.resizeHandlers[id] = handler;
        window.addEventListener('resize', handler);
    },
    removeResizeHandler: function (id) {
        var handler = window.gridJsFunctions.resizeHandlers[id];
        if (handler) {
            window.removeEventListener('resize', handler);
            delete window.gridJsFunctions.resizeHandlers[id];
        }
    },
    // A clock column is taller than the frame that holds it, so a value past the first few
    // rows opens out of sight and the reader is looking at a control that seems to have
    // forgotten what it holds. Centred rather than merely visible, so the values either side
    // stay readable and it is obvious the list continues.
    scrollSelectedIntoView: function (container) {
        if (!container) return;
        var columns = container.querySelectorAll('.grid-timepicker-column');
        [].forEach.call(columns, function (column) {
            var item = column.querySelector('.grid-timepicker-selected');
            if (!item) return;
            var target = item.offsetTop - column.offsetTop - (column.clientHeight - item.offsetHeight) / 2;
            column.scrollTop = target > 0 ? target : 0;
        });
    },
    getPosition: function (element) {
        if (element) {
            var width = element.offsetWidth;
            var height = element.offsetHeight;
            // Read before the walk below, which leaves element at null: asked afterwards this
            // threw on every call, the catch swallowed it, and the answer was "ltr" however the
            // page was written.
            var direction = "ltr";
            try { direction = window.getComputedStyle(element).direction || "ltr"; } catch (e) { }
            var x = 0;
            var y = 0;
            while (element && !isNaN(element.offsetLeft) && !isNaN(element.offsetTop)) {
                x += element.offsetLeft - element.scrollLeft;
                y += element.offsetTop - element.scrollTop;
                element = element.offsetParent;
            }
            x -= Math.round(window.pageXOffset);
            y -= Math.round(window.pageYOffset);
            var screenHeight = screen.availHeight;
            var screenWidth = screen.availWidth;
            var innerHeight = window.innerHeight;
            var innerWidth = window.innerWidth;
            return {
                Direction: direction,
                Width: Math.round(width), Height: Math.round(height), X: Math.round(x), Y: Math.round(y),
                ScreenWidth: Math.round(screenWidth), ScreenHeight: Math.round(screenHeight),
                InnerWidth: Math.round(innerWidth), InnerHeight: Math.round(innerHeight)
            };
        }
        else
            return null;
    },
    setItemActive: function (element, i, activeClass) {
        if (element) {
            var elements = element.querySelectorAll('[data-grid="tab-item"]');
            [].forEach.call(elements, function (el) {
                if (activeClass)
                    el.classList.remove(activeClass);
            });
            elements = element.querySelectorAll('[data-grid-id="' + i + '"]');
            [].forEach.call(elements, function (el) {
                if (activeClass)
                    el.classList.add(activeClass);
            });
        }
    },
    setLinkActive: function (element, i, activeClass) {
        if (element) {
            var elements = element.querySelectorAll('[data-grid="tab-link"]');
            [].forEach.call(elements, function (el) {
                if (activeClass)
                    el.classList.remove(activeClass);
            });
            elements = element.querySelectorAll('[data-grid-id="' + i + '"]');
            [].forEach.call(elements, function (el) {
                if (activeClass)
                    el.classList.add(activeClass);
            });
        }
    },
    setPaneActive: function (element, i, activeClass, hiddenClass) {
        if (element) {
            var elements = element.querySelectorAll('[data-grid="tab-pane"]');
            [].forEach.call(elements, function (el) {
                if (activeClass)
                    el.classList.remove(activeClass);
                if (hiddenClass)
                    el.classList.add(hiddenClass);
            });
            elements = element.querySelectorAll('[data-grid-id="' + i + '"]');
            [].forEach.call(elements, function (el) {
                if (hiddenClass)
                    el.classList.remove(hiddenClass);
                if (activeClass)
                    el.classList.add(activeClass);
            });
        }
    },
    saveAsFile: function (filename, bytesBase64) {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Needed for Firefox
        link.click();
        document.body.removeChild(link);
    },
    click: function (element) {
        if (element)
            element.click();
    },
    showElement: function (element) {
        if (element)
            element.style.display = "block";
    },
    hideElement: function (element) {
        if (element)
            element.style.display = "none";
    },
    scrollFixedSizeTable: function (gridTableHead, gridTableBody, gridTableTotals) {
        document.getElementById(gridTableBody).onscroll = function () {
            document.getElementById(gridTableHead).scrollLeft = this.scrollLeft;
            var elem = document.getElementById(gridTableTotals);
            if (elem)
                elem.scrollLeft = this.scrollLeft;
        };
    }
}