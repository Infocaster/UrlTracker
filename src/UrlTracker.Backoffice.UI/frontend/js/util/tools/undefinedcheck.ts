export function ensureNotUndefined<T extends {}>(obj: T | undefined, msg: string = "Required object is undefined"): asserts obj is T {
    
    if (!obj) throw Error(msg);
}

export function ensureServiceNotUndefined<T extends {}>(obj: T | undefined, service: string): asserts obj is T {

    ensureNotUndefined(obj, `This element requires an instance of ${service}`);
}