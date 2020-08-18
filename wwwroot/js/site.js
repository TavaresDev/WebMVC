// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




//---------------- Script to pupulate the subCategory list ----------------//

//let categoryList = document.getElementById("ddlCategorylist");

////Event: On load
//document.addEventListener("DOMContentLoaded", updateSubCategoryList);
////Event: On change
//categoryList.addEventListener("change", updateSubCategoryList);

function updateSubCategoryList() {
    let categorySelected = document.getElementById("ddlCategorylist").value;
    let list = document.getElementById("SubcategoryList");
    let path = window.location.pathname

    //ugly fix...Works thoug
    console.log(path)
    if (path == '/SubCategory/Create') {
        pathFetch = "../SubCategory/getSubCategory/" + categorySelected
    } else {
        pathFetch = "../getSubCategory/" + categorySelected
    }

   //create "/SubCategory/Create"
   // edit /SubCategory/Edit/5"


    fetch(pathFetch)
        .then(response => {
            console.log(response);
            return response.text();
        })
        .then(data => {
            let results = JSON.parse(data);
            list.innerHTML = " "

            for (i in results) {

                list.innerHTML += `<p class="text-muted">  ${results[i].text} </p>`
            }

        });
};
