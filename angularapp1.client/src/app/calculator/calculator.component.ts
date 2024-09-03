import { HttpClient, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatGridListModule } from '@angular/material/grid-list';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { BehaviorSubject, Subject, Observable, of, combineLatest, Subscription } from 'rxjs';
import { switchMap, catchError, map, takeUntil, startWith, filter } from 'rxjs/operators';
import { AbbreviateNumberPipe } from '../abbreviate-number.pipe';

@Component({
  selector: 'app-calculator',
  templateUrl: './calculator.component.html',
  styleUrls: ['./calculator.component.css'],
  standalone: true,
  imports: [
    MatCardModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatToolbarModule,
    MatIconModule,
    MatDividerModule,
    MatGridListModule,
    FormsModule,
    CommonModule,
    AbbreviateNumberPipe
  ]
})
export class CalculatorComponent implements OnInit, OnDestroy {
  operation$ = new BehaviorSubject<string>('');
  result$ = new BehaviorSubject<string | null>(null);
  isLoading$ = new BehaviorSubject<boolean>(false);
  private input$ = new Subject<string>();
  private destroy$ = new Subject<void>();
  private requestSubscription: Subscription | null = null;
  buttons = ['7', '8', '9', '/', '4', '5', '6', '*', '1', '2', '3', '-', '0', '.', '=', '+', '!'];

  displayValue$ = combineLatest([this.operation$, this.result$]).pipe(
    map(([operation, result]) => this.sanitize(result || operation || '0'))
  );

  constructor(private sanitizer: DomSanitizer, private http: HttpClient) { }

  ngOnInit(): void {
    this.input$.pipe(
      takeUntil(this.destroy$),
      switchMap(value => {
        if (this.requestSubscription) {
          this.requestSubscription.unsubscribe();
          this.requestSubscription = null;
        }

        if (this.result$.getValue() !== null && value !== 'C' && value !== '=') {
          this.operation$.next(value);
          this.result$.next(null);
          return of();
        }

        return value === '=' ? this.calculate() : this.updateOperation(value);
      })
    ).subscribe();
  }

  onButtonClick(value: string): void {
    this.input$.next(value);
  }

  clearDisplay(): void {
    this.reset();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.requestSubscription) {
      this.requestSubscription.unsubscribe();
    }
  }

  private calculate(): Observable<void> {
    this.isLoading$.next(true);
    this.requestSubscription = this.http.post<{ result: string }>('/api/Calculator/calculate', { operation: this.operation$.getValue() })
      .pipe(
        map(response => {
          this.result$.next(this.sanitize(response.result || 'Error'));
          this.operation$.next('');
          this.isLoading$.next(false);
        }),
        catchError((error: HttpErrorResponse) => {
          this.result$.next('Error');
          this.isLoading$.next(false);
          return of();
        })
      ).subscribe();

    return of();
  }

  private updateOperation(value: string): Observable<void> {
    const operation = this.operation$.getValue() + value;
    this.operation$.next(operation);
    return of();
  }

  private reset(): void {
    this.operation$.next('');
    this.result$.next(null);
    this.isLoading$.next(false);
    if (this.requestSubscription) {
      this.requestSubscription.unsubscribe();
    }
  }

  private sanitize(text: string): string {
    return this.sanitizer.sanitize(1, text) || 'Error';
  }
}
