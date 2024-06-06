export function downloadBlob(response: Blob | File, fileName: string): void {
  const blob = new Blob([response], { type: 'text/csv;charset=utf-8;' });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.setAttribute('download', fileName + '.csv');
  document.body.appendChild(link);
  link.click();
}
