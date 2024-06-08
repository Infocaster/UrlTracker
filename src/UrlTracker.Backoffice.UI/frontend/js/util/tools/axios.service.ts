import axios, { AxiosResponseTransformer } from 'axios';

export function initialiseAxios() {
  axios.defaults.transformResponse = prependTransform(axios.defaults.transformResponse, deserializeJsonWithDateReviver);
}

function prependTransform(
  defaults: AxiosResponseTransformer | AxiosResponseTransformer[] | undefined,
  transform: AxiosResponseTransformer,
): AxiosResponseTransformer[] {
  defaults = ensureDefaultsAsArray(defaults);
  return [transform, ...defaults];
}

function ensureDefaultsAsArray(
  defaults: AxiosResponseTransformer | AxiosResponseTransformer[] | undefined,
): AxiosResponseTransformer[] {
  return Array.isArray(defaults) ? defaults : !defaults ? [] : [defaults];
}

function deserializeJsonWithDateReviver(data: string) {
  try {
    const parsedData = JSON.parse(data, dateStringToDateReviver);
    return parsedData;
  } catch (e) {
    return data;
  }
}

function dateStringToDateReviver(_: string, value: unknown) {
  if (typeof value !== 'string' && !(value instanceof String)) return value;

  const valueToString = value.toString();
  const matchesExpectedPattern = /^\d{4}-\d{2}-\d{2}.*/.test(valueToString);
  const valueAsDate = Date.parse(valueToString);
  if (!matchesExpectedPattern || isNaN(valueAsDate)) return value;

  return new Date(valueAsDate);
}
