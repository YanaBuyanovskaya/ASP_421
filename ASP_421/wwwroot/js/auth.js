document.getElementById('auth-form')?.addEventListener('submit', async (e) => {
    e.preventDefault();

    const form = e.currentTarget;
    const errors = document.getElementById('auth-errors');
    errors.classList.add('d-none');
    errors.innerHTML = '';

    const login = form.querySelector('input[name="login"]').value ?? '';
    const password = form.querySelector('input[name="password"]').value ?? '';

    const raw = `${login}:${password}`;


    const resp = await fetch(form.action, {
        method: 'POST',
        headers: {
            'Authorization': `Basic ${basic}`,
            'X-Requested-With': 'XMLHttpRequest'
        }
    });

    if (resp.ok) {
        const modal = bootstrap.Modal.getOrCreateInstance('#authModal');
        modal.hide();
        location.reload();
        return;
    }

    try {
        const json = await resp.json();
        if (json?.errors) {
            const list = [];
            for (const k in json.errors) list.push(...json.errors[k]);
            errors.innerHTML = list.map(x => `<div>${x}</div>`).join('');
        }
        else if (json?.message) {
            errors.textContent = json.message;
        }
        else {
            errors.textContent = 'Помилка в автентифікації';

        }
    }
catch {
            errors.textContent = 'Сталася помилка запиту';
        }
        errors.classList.remove('d-none');

    });

