import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { LoanService, Loan, LoanHistory } from './services/loan.service';
import { AuthService, AuthResponse } from './services/auth.service';
import { LoginComponent } from './components/login/login.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    MatTableModule, 
    MatButtonModule, 
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    LoginComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  displayedColumns: string[] = [
    'loanAmount',
    'currentBalance',
    'applicant',
    'status',
    'actions'
  ];

  historyDisplayedColumns: string[] = [
    'loanAmount',
    'applicant',
    'status',
    'snapshotDate',
    'changeType',
    'paymentAmount'
  ]
  
  loans: Loan[] = [];
  isLoading = false;
  error: string | null = null;
  isAuthenticated = false;
  currentUser: AuthResponse | null = null;

  loanHistory: LoanHistory[] = [];
  historyLoading = false;
  historyError: string | null = null;

  constructor(
    private loanService: LoanService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    // Subscribe to authentication state
    this.authService.token$.subscribe(token => {
      this.isAuthenticated = !!token;
      if (this.isAuthenticated) {
        this.loadLoans();
        this.loadLoanHistory();
      } else {
        this.loans = [];
      }
    });

    this.authService.user$.subscribe(user => {
      this.currentUser = user;
    });
  }

  loadLoans() {
    this.isLoading = true;
    this.error = null;
    
    this.loanService.getAllLoans().subscribe({
      next: (loans) => {
        this.loans = loans;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading loans:', error);
        this.error = 'Failed to load loans. Please try again.';
        this.isLoading = false;
        this.showError('Failed to load loans');
      }
    });
  }

  loadLoanHistory () {
    this.historyLoading = true;
    this.historyError = null;
    
    this.loanService.getLoanHistory(1).subscribe({
      next: (history) => {
        this.loanHistory = history;
        this.historyLoading = false;
      },
      error: (error) => {
        console.error('Error loading loan history:', error);
        this.historyError = 'Failed to load loan history. Please try again.';
        this.historyLoading = false;
        this.showError('Failed to load loan history');
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.showSuccess('Logged out successfully');
  }

  makePayment(loan: Loan, amount: number) {
    if (amount <= 0 || amount > loan.currentBalance) {
      this.showError('Invalid payment amount');
      return;
    }

    this.loanService.makePayment(loan.id, { amount }).subscribe({
      next: (updatedLoan) => {
        const index = this.loans.findIndex(l => l.id === updatedLoan.id);
        if (index !== -1) {
          this.loans[index] = updatedLoan;
        }
        this.showSuccess(`Payment of $${amount} processed successfully`);
      },
      error: (error) => {
        console.error('Error making payment:', error);
        this.showError('Failed to process payment');
      }
    });
  }

  quickPayment(loan: Loan, percentage: number) {
    const amount = Math.round(loan.currentBalance * percentage / 100 * 100) / 100;
    this.makePayment(loan, amount);
  }

  private showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleString();
  }
}
