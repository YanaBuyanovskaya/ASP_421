(() => {
    'use strict';
    // prevent double init if included twice
    if (window.__cartInit) return; window.__cartInit = true;

    const box = document.getElementById('cart-msg') || (() => {
        const d = document.createElement('div');
        d.id = 'cart-msg';
        d.className = 'position-fixed bottom-0 end-0 p-3';
        d.style.zIndex = 1080;
        document.body.appendChild(d);
        return d;
    })();

    const csrf = document.querySelector('meta[name="csrf-token"]')?.content || null;
    const badge = document.querySelector('[data-cart-badge]');

    function toast(text, type) {
        const el = document.createElement('div');
        el.className = `alert alert-${type} shadow`;
        el.textContent = text;
        box.appendChild(el);
        setTimeout(() => el.remove(), 3000);
    }

    async function initBadge() {
        try {
            const res = await fetch('/api/cart/summary', {
                headers: csrf ? { 'RequestVerificationToken': csrf } : {}
            });
            if (!res.ok) return;
            const js = await res.json();
            if (badge && typeof js.totalQty === 'number') badge.textContent = js.totalQty;
        } catch {  }
    }

    async function addToCart(productId) {
        try {
            const res = await fetch(`/api/cart/${productId}`, {
                method: 'POST',
                headers: csrf ? { 'RequestVerificationToken': csrf } : {}
            });

            if (res.status === 401) { toast('Для замовлення необхідно увійти в систему', 'warning'); return; }
            if (!res.ok) { toast('Виникла помилка додавання, повторіть спробу пізніше', 'danger'); return; }

            const data = await res.json().catch(() => ({}));
            toast('Товар успішно додано', 'success');
            if (badge && typeof data.totalQty === 'number') badge.textContent = data.totalQty;
        } catch {
            toast('Виникла помилка додавання, повторіть спробу пізніше', 'danger');
        }
    }

    document.addEventListener('click', (e) => {
        const btn = e.target.closest('.btn-add-to-cart');
        if (!btn) return;
        const id = btn.getAttribute('data-product-id');
        if (id) addToCart(id);
    });

    // init on load
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initBadge, { once: true });
    } else {
        initBadge();
    }
})();

