export function toggleNav() {
    const navMenu = document.querySelector('.hl-nav-left');
    const element = document.querySelector('.hl-disabler');

    if (navMenu.classList.contains("hl-nav-left-open")) {
        closeNav()
    }
    else {
        openNav();
    }
}

export function openNav() {
    const navMenu = document.querySelector('.hl-nav-left');
    const element = document.querySelector('.hl-disabler');

    if (!navMenu.classList.contains("hl-nav-left-open")) {

        document.body.style.overflow = "hidden";

        navMenu.classList.add("hl-nav-left-open");
        element.classList.remove('d-none');

        setTimeout(() => {
            element.classList.add("hl-disabled");
        }, 10);
    }
}

export function closeNav() {
    const navMenu = document.querySelector('.hl-nav-left');
    const element = document.querySelector('.hl-disabler');

    if (navMenu.classList.contains("hl-nav-left-open")) {

        document.body.style.overflow = "auto";

        navMenu.classList.remove("hl-nav-left-open");

        element.classList.remove('hl-disabled');
        element.addEventListener('transitionend', () => {
            element.classList.add('d-none');
        }, { once: true });
    }
}