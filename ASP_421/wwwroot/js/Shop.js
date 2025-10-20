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
    for (let btn of document.querySelectorAll('[data-goto="cart"]')) {
        btn.addEventListener('click', gotoToCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-item]')) {
        btn.addEventListener('click', modifyCartItemClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-checkout]')) {
        btn.addEventListener('click', checkoutCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-cancel]')) {
        btn.addEventListener('click', cancelCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-repeat]')) {
        btn.addEventListener('click', repeatCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-restore]')) {
        btn.addEventListener('click', restoreCartClick);
    }
    for (let btn of document.querySelectorAll('[data-remove-from-cart]')) {
        btn.addEventListener('click', removeItemFromCartClick);
    }
});

function removeItemFromCartClick(e) {
    e.preventDefault();
    let btn = e.target.closest("[data-remove-from-cart]");
    let cartId = btn.getAttribute("data-remove-from-cart");
    let inc = Number(btn.getAttribute("data-cart-inc"));
    if (confirm("Видалити дану позицію з кошика?")) {
        fetch("/api/cart/remove/" + cartId, {
            method: "DELETE"
        }).then(r => r.json())
            .then(j => {
                // перевірити, що відповідь успішна
                if (j.code == 200) {
                    alert("Позицію успішно видалено");
                    window.location.reload();
                }
                else {
                    alert("Виникла помилка. Повторіть дія пізніше");
                }
            });
        }
}
function repeatCartClick(e) {
    e.preventDefault();
    let btn = e.target.closest("[data-cart-repeat]");
    let cartId = btn.getAttribute("data-cart-repeat");
    if (confirm("Повторити замовлення?")) {
        fetch("/api/cart/repeat/" + cartId, {
            method: "POST"
        }).then(r => r.json())
            .then(j => {
                // перевірити, що відповідь успішна
                if (j.code == 200) {
                    window.location = "/Shop/Cart";
                }
                else {
                    alert("Виникла помилка. Повторіть дія пізніше");
                }

            });
    }
}


function restoreCartClick(e) {
    e.preventDefault();
    let btn = e.target.closest("[data-cart-restore]");
    let cartId = btn.getAttribute("data-cart-restore");
    if (confirm("Відновити замовлення?")) {
        fetch("/api/cart/restore/" + cartId, {
            method: "POST"
        }).then(r => r.json())
            .then(j => {
                // перевірити, що відповідь успішна
                if (j.code == 200) {
                    window.location = "/Shop/Cart";
                }
                else {
                    alert("Виникла помилка. Повторіть дія пізніше");
                }

            });
    }
}
    function checkoutCartClick(e) {
        e.preventDefault();
        //let btn = e.target.closest("[data-cart-checkout]");
        //let cardId = btn.getAttribute("data-cart-checkout");
        if (confirm("Оформлюємо покупку?")) {
            fetch("/api/cart/", {
                method: "PUT"
            }).then(r => r.json())
                .then(j => {
                    // перевірити, що відповідь успішна
                    if (j.code == 200) {
                        alert("Дякуємо за покупку!");
                        window.location = "/Shop";
                    }
                    else {
                        alert("Виникла помилка. Повторіть дія пізніше");
                    }

                });
        }
    }

    function cancelCartClick(e) {
        e.preventDefault();
        let btn = e.target.closest("[data-cart-cancel]");
        let cartId = btn.getAttribute("data-cart-cancel");
        if (confirm("Скасувати покупку?")) {
            fetch("/api/cart/", {
                method: "DELETE"
            }).then(r => r.json())
                .then(j => {
                    // перевірити, що відповідь успішна
                    if (j.code == 200) {
                        alert("Кошик видалено");
                        window.location = "/Shop";
                    }
                    else {
                        alert("Виникла помилка. Повторіть дія пізніше");
                    }

                });
        }
    }

    function modifyCartItemClick(e) {
        e.preventDefault();
        let btn = e.target.closest("[data-cart-item]");
        let cartItemId = btn.getAttribute("data-cart-item");
        let inc = Number(btn.getAttribute("data-cart-inc") || "1");
        console.log(cartItemId, inc);

        fetch("/api/cart/" + cartItemId + "?inc=" + inc, {
            method: "PATCH"
        }).then(r => r.json())
            .then(j => {
                // перевірити, що відповідь успішна
                window.location.reload();
            });
    }

    function gotoToCartClick(e) {
        e.preventDefault();
        window.location.href = "/Shop/Cart";
    }


    function addToCartClick(e) {
        e.preventDefault();
        let btn = e.target.closest("[data-add-to-cart]");
        let productId = btn.getAttribute("data-add-to-cart");
        console.log(productId);

        fetch("/api/cart/" + productId + "", {
            method: "POST"
        }).then(r => r.json()).then(j => {
            // перевірити, що відповідь успішна
            let pic = document.getElementById("product-in-cart");
            if (pic) pic.innerHTML = '<a href="/Shop/Cart" class="btn btn-outline-primary">Перейти до кошику</a>';
            else window.location.reload();
        });
    }


