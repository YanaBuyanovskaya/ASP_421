class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    static encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    static decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    static encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    static decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    static jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    static jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}
/*
Native:
import { Buffer } from 'buffer';
Buffer.from(user.split('.')[1], 'base64').toString('utf8');
Buffer.from(`${login}:${password}`, 'utf-8').toString('base64');
*/

document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == "auth-form") {
        e.preventDefault();
        const data = new FormData(form);
        const login = data.get("auth-login");
        const password = data.get("auth-password");

        //RFC 7617
        const userPass = login + ':' + password;
        const credentials = Base64.encode(userPass);
        fetch("/User/SignIn", {
            headers: {
                "Authorization": "Basic " + credentials
            }
        }).then(r => r.json()).then(j => {
            if (j.status == 200) {
                window.location.reload()
            }
            else {
                console.log(j);
            }
        });

        console.log(login, password, credentials);
    }
});


document.addEventListener('DOMContentLoaded', e => {
    let btn = document.getElementById("btn-profile-edit");
    if (btn) btn.onclick = btnProfileEditClick;
    btn = document.getElementById("btn-profile-delete");
    if (btn) btn.onclick = btnProfileDeleteClick;
});

function btnProfileEditClick() {
    const elements = document.querySelectorAll("[data-editable]");

    if (elements.length == 0) {
        console.error("Empty [data-editable], returned");
        return;
    }

    if (elements[0].hasAttribute("contenteditable")) {
        let changes = {};
        let wasChanges = false;
        for (let elem of elements) {
            elem.removeAttribute("contenteditable");
            if (elem.originText != elem.innerText) {
                changes[elem.getAttribute("data-editable")] = elem.innerText;
                wasChanges = true;
            }
        }
        if (wasChanges) {
            if (confirm("Підтверджуєте внесення зміе?" + JSON.stringify(changes))) {
                console.log(changes);
                fetch("/User/Update", {
                    method: "PATCH",
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(changes)
                }).then(r => r.json()).then(j => {
                    if (j.status == 200) {
                        alert("Дані оновлено");
                    }
                    else {
                        alert("Помилка оновлення: " + j.data);
                    }
                });

            }
            else {
                for (let elem of elements) {
                    elem.innerText = elem.originText;
                }
            }
        }
    }
    else {
        for (let elem of elements) {
            elem.setAttribute("contenteditable", true);
            //зберігаємо значення, що було перед редагуванням
            elem.originText = elem.innerText;
            
        }
    }

    function btnProfileDeleteClick() {

    }
}
