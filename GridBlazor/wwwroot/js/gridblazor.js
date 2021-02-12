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
    setActive: function (element, i) {
        if (element) {
            var elements = element.querySelectorAll('[data-grid="tab"]');
            [].forEach.call(elements, function (el) {
                el.classList.remove("active");
            });
            elements = element.querySelectorAll('[data-grid-id="' + i + '"]');
            [].forEach.call(elements, function (el) {
                el.classList.add("active");
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
    scrollFixedSizeTable: function (gridTableHead, gridTableBody) {
        document.getElementById(gridTableBody).onscroll = function () {
            document.getElementById(gridTableHead).scrollLeft = this.scrollLeft;
        };
    }
}