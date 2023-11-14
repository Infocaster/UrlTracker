export function ensureExists<T extends {}>(
  obj: T | undefined,
  msg: string = "Required object is undefined"
): asserts obj is T {
  if (!obj) throw Error(msg);
}

export function ensureServiceExists<T extends {}>(
  obj: T | undefined,
  service: string
): asserts obj is T {
  ensureExists(obj, `This element requires an instance of ${service}`);
}
