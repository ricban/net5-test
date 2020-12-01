export const localStore = {

    set: (key: string, value: string): void => {
        localStorage.setItem(key, value);
    },

    get: (key: string): string => {
        return localStorage.getItem(key);
    },

    remove: (key: string): void => {
        localStorage.removeItem(key);
    },

    clear: (): void => {
        localStorage.clear();
    }

}