var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/Visitations/GetAllVisits/",
            "type": "GET",
            "datatype": "json"
        },
        "dom": "tr",
        "columns": [
            { "data": "user" },
            { "data": "doctor" },
            { "data": "date" },
            { "data": "type" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/Visitation/Details?id=${data}" class='btn btn-success text-white' style='cursor:pointer;'>
                            <i class="fa fa-book"></i>
                        </a>`;
                }, "width": "15%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%",
        initComplete: function () {
            this.api().columns().every(function () {
                var that = this;
                $('#' + $(this.footer()).text()).on('keyup change clear', function () {
                    if (that.search() !== this.value.trim()) {
                        that
                            .search(this.value.trim())
                            .draw();
                    }
                });
            });
        },
    });
}