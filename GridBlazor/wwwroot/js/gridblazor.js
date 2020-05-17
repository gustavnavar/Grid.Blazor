﻿window.gridJsFunctions = {
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
            var screenHeight = screen.availHeight;
            var screenWidth = screen.availWidth;
            var innerHeight = window.innerHeight;
            var innerWidth = window.innerWidth;
            return {
                Width: width, Height: height, X: x, Y: y, ScreenWidth: screenWidth, ScreenHeight: screenHeight,
                InnerWidth: innerWidth, InnerHeight: innerHeight
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
    }
}