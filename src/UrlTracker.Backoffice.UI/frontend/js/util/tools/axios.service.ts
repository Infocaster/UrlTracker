import { Axios } from 'axios';
import Cookies from 'js-cookie';

export const axiosInstance = new Axios({
  transformResponse: [
    (data) => {
      try {
        const parsedData = JSON.parse(data.substring(6), dateStringToDateReviver);
        return parsedData;
      } catch (e) {
        console.warn('Could not parse response', e);
        return data;
      }
    },
  ],
  transformRequest: [
    (data) => {
      if (data instanceof FormData) {
        return data;
      }
      return JSON.stringify(data);
    },
  ],
  headers: {
    'Content-Type': 'application/json',
    'X-UMB-XSRF-TOKEN': Cookies.get('UMB-XSRF-TOKEN'),
  },
});

function dateStringToDateReviver(_: string, value: any) {
  if (typeof value !== 'string' && !(value instanceof String)) return value;

  const valueToString = value.toString();
  const matchesExpectedPattern = /^\d{4}-\d{2}-\d{2}.*/.test(valueToString);
  const valueAsDate = Date.parse(valueToString);
  if (!matchesExpectedPattern || isNaN(valueAsDate)) return value;

  return new Date(valueAsDate);
}
