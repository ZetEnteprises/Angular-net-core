import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'abbreviateNumber',
  standalone: true
})
export class AbbreviateNumberPipe implements PipeTransform {

  transform(value: string | number): string {
    const num = Number(value);
    if (isNaN(num)) return value.toString();
    if (num >= 1e15) return num.toExponential(15);
    if (num >= 1e12) return num.toExponential(12);
    if (num >= 1e9) return num.toExponential(9);
    if (num >= 1e6) return num.toExponential(6);
    if (num >= 1e3) return num.toExponential(3);

    return num.toString();
  }
}
