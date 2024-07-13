var dbtl;

$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dbtl = $("#users").DataTable({
        "ajax": {
            "url": "/Admin/Users/GetData",
            "dataSrc": "data" // Specify the data source property
        },

        "columns": [
            { "data": "firstName" },
            { "data": "lastName" },
            { "data": "email" },
            { "data": "phoneNumber" },
            { "data": "role" },
            {
                "data": "id",
                "orderable": false,
                "searchable": false,
                "render": function (data, type, row) {
                    // Check if the user is not an admin
                    if (row.role.toLowerCase() != "admin") {
                        if (row.locked == true) {
                            return `<a href="/Admin/Users/LockUnlock/${row.id}" class="btn btn-success"><i class="fas fa-lock-open"></i></a>`;
                        } else {
                            return `<a href="/Admin/Users/LockUnlock/${row.id}" class="btn btn-danger"><i class="fas fa-lock"></i></a>`;
                        }
                    } else {
                        return ''; // Return empty string if the role is admin
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a onclick=DeleteItem("/Admin/Product/Delete/${data}") class="btn btn-danger">Delete</a>`;
                }
            }
        ]


    });
}




function DeleteItem(url) {

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dbtl.ajax.reload();
                        toaster.success(data.message);
                    }
                    else {
                        toaster.error(data.message);
                    }

                }

            });


            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });

}


