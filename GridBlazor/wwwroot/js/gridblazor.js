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
    getPosition: function (element) {
        if (element) {
            var width = element.offsetWidth;
            var height = element.offsetHeight;
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