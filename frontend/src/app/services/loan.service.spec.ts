import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LoanService, Loan, CreateLoanRequest } from './loan.service';
import { AuthService } from './auth.service';

describe('LoanService', () => {
  let service: LoanService;
  let httpMock: HttpTestingController;
  let mockAuthService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    mockAuthService = jasmine.createSpyObj('AuthService', ['getAuthHeaders']);
    mockAuthService.getAuthHeaders.and.returnValue({
      'Authorization': 'Bearer mock-token',
      'Content-Type': 'application/json'
    });

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        LoanService,
        { provide: AuthService, useValue: mockAuthService }
      ]
    });
    service = TestBed.inject(LoanService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all loans', () => {
    const mockLoans: Loan[] = [
      {
        id: 1,
        amount: 1000,
        currentBalance: 500,
        applicantName: 'John Doe',
        status: 'Active',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
    ];

    service.getAllLoans().subscribe(loans => {
      expect(loans).toEqual(mockLoans);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/LoanManagement/loans`);
    expect(req.request.method).toBe('GET');
    req.flush(mockLoans);
  });

  it('should create a loan', () => {
    const createRequest: CreateLoanRequest = {
      amount: 1000,
      applicantName: 'John Doe'
    };

    const mockLoan: Loan = {
      id: 1,
      amount: 1000,
      currentBalance: 1000,
      applicantName: 'John Doe',
      status: 'Active',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    service.createLoan(createRequest).subscribe(loan => {
      expect(loan).toEqual(mockLoan);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/LoanManagement/loans`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(mockLoan);
  });

  it('should make a payment', () => {
    const loanId = 1;
    const paymentRequest = { amount: 100 };
    const mockUpdatedLoan: Loan = {
      id: 1,
      amount: 1000,
      currentBalance: 900,
      applicantName: 'John Doe',
      status: 'Active',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };

    service.makePayment(loanId, paymentRequest).subscribe(loan => {
      expect(loan).toEqual(mockUpdatedLoan);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/LoanManagement/loans/${loanId}/payment`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(paymentRequest);
    req.flush(mockUpdatedLoan);
  });
}); 