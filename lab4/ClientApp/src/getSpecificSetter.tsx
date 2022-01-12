export default function getSpecificSetter<T>(obj: T, generalSetter: Function, key: string) {
    return ((value: typeof obj[keyof T]) => generalSetter((prevValue: T) => ({ ...prevValue, [key]: value })));
}
