import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { AuthService } from './auth.service';

export interface Loan {
  id: number;
  amount: number;
  currentBalance: number;
  applicantName: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface LoanHistory {
  id: number;
  loanId: number;
  amount: number;
  applicantName: string;
  status: string;
  createdAt: string;
  updatedAt: string;
  snapshotDate: string;
  changeType: string;
  paymentAmount?: number;
}

export interface CreateLoanRequest {
  amount: number;
  applicantName: string;
}

export interface PaymentRequest {
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private readonly apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getAllLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.apiUrl}/loans`, {
      headers: this.getAuthHeaders()
    });
  }

  getLoanById(id: number): Observable<Loan> {
    return this.http.get<Loan>(`${this.apiUrl}/loans/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  createLoan(request: CreateLoanRequest): Observable<Loan> {
    return this.http.post<Loan>(`${this.apiUrl}/loans`, request, {
      headers: this.getAuthHeaders()
    });
  }

  makePayment(id: number, request: PaymentRequest): Observable<Loan> {
    return this.http.post<Loan>(`${this.apiUrl}/loans/${id}/payment`, request, {
      headers: this.getAuthHeaders()
    });
  }

  getLoanHistory(id: number): Observable<LoanHistory[]> {
    return this.http.get<LoanHistory[]>(`${this.apiUrl}/loans/${id}/history`, {
      headers: this.getAuthHeaders()
    });
  }
} 