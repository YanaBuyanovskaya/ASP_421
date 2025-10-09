document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == "admin-group-form") {
        e.preventDefault();
        fetch("/api/group", {
            method: "POST",
            body: new FormData(form)
        }).then(r => r.json()).then(console.log);
    }
    if (form.id == "admin-product-form") {
        e.preventDefault();
        fetch("/api/product", {
            method: "POST",
            body: new FormData(form)
        }).then(r => r.json()).then(console.log);
    }
}); 

document.addEventListener('DOMContentLoaded', () => {
    for (let btn of document.querySelectorAll("[data-add-to-cart]")) {
        btn.addEventListener('click', addToCartClick);
    }
});

function addToCartClick(e) {
    e.preventDefault();
    let btn = e.target.closest("[data-add-to-cart]");
    let productId = btn.getAttribute("data-add-to-cart");
    console.log(productId);
    fetch("/api/cart/" + productId, {
        method: "POST",
    }).then(r => r.json()).then(console.log);
}
