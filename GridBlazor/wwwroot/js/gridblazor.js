window.gridJsFunctions = {
    focusElement: function (element) {
        if(element)
            element.focus();
    },
    isDateTimeLocalSupported: function () {
        var elem = document.createElement("input");
        elem.setAttribute("type", "datetime-local");
        return elem.type !== "text";
    }
}