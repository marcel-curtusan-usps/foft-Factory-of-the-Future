//connection types
let connectiontypetable = "connectiontypetable";


$.extend(fotfmanager.client, {
    updateConnectionType: async (update) => { Promise.all([updateConnectiontypeDataTable(update, connectiontypetable)]); },
    removeConnectionType: async (remove) => { Promise.all([removeConnection(remove)]); },
    addConnectionType: async (add) => { Promise.all([addConnection(add)]); }
});
async function init_connectiontType(data) {
    try {
        Promise.all([createConnectiontypeDataTable(connectiontypetable)]);
        if (data.length > 0) {
            Promise.all([connectionTypeLoad(data), updateConnectiontypeDataTable(data, connectiontypetable)]);
        }
    } catch (e) {
    }
}
function format(rowData) {
    return '<table id="' + rowData.Id + '_subconntypetable" >' + '</table>';
}
async function createConnectiontypeDataTable(table) {
    let arrayColums = [{
        "Name": "",
        "Description": "",
        "Action": ""
    }]
    let columns = [];
    let tempc = {};
    tempc = {
        "className": 'details-control',
        "orderable": false,
        "defaultContent": '',
        "data": null
    }
    columns.push(tempc);
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/Name/i.test(key)) {
            tempc = {
                "title": 'Name',
                "width": "30%",
                "mDataProp": key
            }
        }
        if (/Description/i.test(key)) {
            tempc = {
                "title": "Description",
                "width": "50%",
                "mDataProp": key
            }
        }
        if (/Action/i.test(key)) {
            tempc = {
                "title": "Action",
                "width": "10%",
                "mDataProp": key,
                "mRender": function () {
                    return '<button class="btn btn-light btn-sm mx-1 pi-iconEdit editconnectiontype" data-toggle="modal" name="editconnectiontype"></button>';
                }
            }
        }
        columns.push(tempc);

    });
    let ConnectiontypeTable = $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        bpaging: false,
        bPaginate: false,
        autoWidth: false,
        bInfo: false,
        destroy: true,
        language: {
            emptyTable: "No Data Available"
        },
        aoColumns: columns,
        columnDefs: [
        ],
        sorting: [[0, "asc"]]

    })
    // Add event listener for opening and closing first level child details
    $('#' + table + ' tbody').off().on('click', 'td.details-control', function () {
        let tr = $(this).closest('tr');
        let row = ConnectiontypeTable.row(tr);
        let rowData = row.data();

        let dataArray = [];
        $.each(rowData.MessageTypes, function () {
            dataArray.push(this);
        });

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            row.child(format(rowData)).show();
            let tabel_id = rowData.Id + "_subconntypetable";
            createConnectiontypeSubtable(tabel_id, dataArray);
            tr.addClass('shown');
        }
    });
    // Edit/remove record
    $('#' + table + ' tbody').on('click', 'button', function () {
        let td = $(this);
        let table = $(td).closest('table');
        let row = $(table).DataTable().row(td.closest('tr'));
        if (/editconnectiontype/ig.test(this.name)) {
            Edit_Connectiontype(row.data());
        }
    });
}
async function createConnectiontypeSubtable(table, row_data) {
    try {
        let arrayColums = [{
            "Code": "",
            "Description": "",
            "Action":""
        }]
        let columns = [];
        let tempc = {};
        $("#error_subconntypetable").text("");
        $.each(arrayColums[0], function (key, value) {
            tempc = {};
            if (/Description/i.test(key)) {
                tempc = {
                    "title": "Description",
                    "mDataProp": key
                }
            }
            if (/Code/i.test(key)) {
                tempc = {
                    "title": "Code",
                    "mDataProp": key
                }
            }
            if (/Action/i.test(key)) {
                tempc = {
                    "title": "Action",
                    "width": "10%",
                    "mDataProp": key,
                    "mRender": function () {
                        return '<button class="btn btn-light btn-sm mx-1 pi-iconEdit editconnectiontype" data-toggle="modal" name="editconnectiontype"></button>';
                    }
                }
            }
            columns.push(tempc);
        });
        $('#' + table).DataTable({
            data: row_data,
            searching: false,
            info: false,
            autoWidth: true,
            destroy: true,
            bpaging: false,
            bPaginate: false,
            select: false,
            aoColumns: columns,
            columnDefs: [],
            sorting: [[0, "asc"]],
        });
        $('#' + table).css('width', '95%')

    } catch (e) {
        console.log(e);
    }
}
async function connectionTypeLoad(connName) {
    try {
        if (!$.isEmptyObject(connName)) {
            $('<option/>').val("blank").html("").appendTo('#connection_name');
            $('<option data-messagetype=blank>').val("").html("").appendTo('#message_type');
            $.each(connName, function (key, value) {
                $('<option/>').val(this.Name).html(this.Description + " (" + this.Name + ")").appendTo('#connection_name');
                let name = this.Name;
                $(this.MessageTypes).each(function (key, value) {
                    $('<option data-messagetype=' + name + '>').val(this.Code).html(this.Description).appendTo('#message_type');

                });
            });
        };

    } catch (e) {
        console.log(e);
    }
}
async function loadConnectiontypeDatatable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows.add(data).draw();
    }
}
async function updateConnectiontypeDataTable(newdata, table) {
    let loadnew = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            loadnew = false;
            $('#' + table).DataTable().row(node).data(element).draw().invalidate();

        })
        if (loadnew) {
            loadConnectiontypeDatatable(newdata, table);
        }
    }
}