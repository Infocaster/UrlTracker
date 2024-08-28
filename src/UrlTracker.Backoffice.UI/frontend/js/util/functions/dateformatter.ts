const dateTimeFormatter = new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' });
const dateOnlyFormatter = new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: undefined });

export const toReadableDate = (date: Date) => dateTimeFormatter.format(date);
export const toReadableDateOnly = (date: Date) => dateOnlyFormatter.format(date);
