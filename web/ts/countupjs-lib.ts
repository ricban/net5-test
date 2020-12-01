import { CountUp, CountUpOptions } from "../../node_modules/countup.js/dist/countUp";

export function countUpValue(elementId: string, endValue: number, options: CountUpOptions): void {

    const countUp = new CountUp(elementId, endValue, options);

    if (!countUp.error) {
        countUp.start();
    } else {
        console.error(countUp.error);
    }

}