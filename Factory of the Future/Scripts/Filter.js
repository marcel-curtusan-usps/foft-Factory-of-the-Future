$(document).ready(function () {
         $(".filterable .filters input").keyup(function (t) {
            if ("9" != (t.keyCode || t.which)) {
                var e = $(this),
                    n = e.parents(".filterable"),
                    d = n.find(".table"),
                    l = d.find("tbody tr"),
                    f = l.hide().filter(function () {
                            return $(this).find('td').filter(function () {

                                var tdText = $(this).text().toLowerCase(),
                                    inputValue = $('#' + $(this).data('input')).val().toLowerCase();

                                return tdText.indexOf(inputValue) != -1;

                            }).length == $(this).find('td').length;

                    }).show();
            }
        })
});