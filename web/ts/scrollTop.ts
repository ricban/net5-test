"use strict";

export type Position = "right" | "left"

export function scrollTop(bg?: string, position: Position = "right", circle = false, yOffset = 400): void {

    const gototop = document.createElement("div");

    if (bg) {
        gototop.style.background = bg;
    }

    if (circle) {
        gototop.classList.add("circle");
    }

    gototop.classList.add("gototop", position);

    gototop.addEventListener("click", () => {

        globalThis.scrollTo({
            top: 0,
            behavior: "smooth"
        });

    });

    document.documentElement.append(gototop);

    globalThis.addEventListener("scroll", () => {

        if (pageYOffset >= yOffset) {
            gototop.classList.add("visible");
        }
        else {
            gototop.classList.remove("visible");
        }

    });

}