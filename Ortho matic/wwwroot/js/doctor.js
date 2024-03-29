﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/Doctors/GetAllDoctors/",
            "type": "GET",
            "datatype": "json"
        },
        "dom": "tr",
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "doctorDegree" },
            { "data": "doctorSpecialty" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/Doctors/Details?id=${data}" class='btn btn-success text-white' style='cursor:pointer;'>
                            <i class="fa fa-book"></i>
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer;'
                            onclick=Delete('/Doctors/DeleteDoctor?id=${data}')>
                            <i class="fa fa-times"></i>
                        </a>
                        </div>`;
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

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
