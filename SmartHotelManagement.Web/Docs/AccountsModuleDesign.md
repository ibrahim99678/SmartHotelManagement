# Accounts Module Design

## Requirements Analysis
- Manage hotel finances with standardized tracking for income and expenses
- Track expense categories: maintenance/repairs, staff salaries and benefits, housekeeping supplies, laundry, utilities (electricity, water, gas, internet), rent/mortgage, insurance, licenses/taxes, marketing/commissions, payment gateway fees, software subscriptions, furniture/fixtures, training
- Track income sources: room bookings, upsell add-ons (early check-in/late checkout), restaurant/bar, room service, minibar, spa/wellness, conference/banquet halls, parking, laundry, tourism services, penalties/fees, refunds/discounts
- Core features:
  - CRUD for expense and income entries
  - Category management (Expense/Income types, parent-child hierarchy)
  - Payment methods (Cash/Card/BankTransfer/Online) with basic metadata
  - Link entries to Reservations and Guests where relevant
  - Recurring entries (monthly utilities, salaries)
  - Budget tracking per category and period
  - Audit logging (who created/edited/deleted entries)
  - Multi-currency support with conversion snapshot
  - Financial reporting: period totals, category breakdowns, trends, profit/loss
  - Export CSV/PDF

## Database Design
- Option A: Unified ledger table
  - FinancialTransaction
    - TransactionId (PK)
    - Type (enum: Expense/Income)
    - Date (datetime)
    - Amount (decimal(18,2))
    - Currency (string, ISO code)
    - CategoryId (FK -> Category)
    - PaymentMethodId (FK -> PaymentMethod)
    - ReservationId (nullable FK -> Reservations)
    - GuestId (nullable FK -> Guests)
    - Description (nvarchar)
    - IsRecurring (bool), RecurrenceRule (nullable)
    - AttachmentUrl (nullable)
    - CreatedBy, CreatedAt, ModifiedBy, ModifiedAt
  - Category
    - CategoryId (PK)
    - Name (nvarchar)
    - Kind (enum: Expense/Income)
    - ParentCategoryId (nullable self-FK)
    - IsActive (bool)
  - PaymentMethod
    - PaymentMethodId (PK)
    - Name (nvarchar) e.g. Cash, Card, Bank Transfer
    - Provider (nullable), MaskedAccount (nullable)
    - IsActive (bool)
  - Budget
    - BudgetId (PK)
    - PeriodStart, PeriodEnd
    - CategoryId (FK -> Category)
    - Amount (decimal(18,2))
    - Currency (string)
- Option B: Separate tables (if preferred)
  - Expenses (same columns as FinancialTransaction but only Expense)
  - Income (same columns as FinancialTransaction but only Income)
  - Category, PaymentMethod, Budget shared
- Constraints/Indexes
  - FK constraints for Category, PaymentMethod, Reservation, Guest
  - Indexes on Date, CategoryId, Type, ReservationId for reporting
  - Cascade behavior: restrict delete for categories used by transactions

## User Interface Specifications
- Dashboard
  - KPIs: MTD Income, MTD Expenses, Net Profit/Loss, Outstanding dues
  - Charts: Income vs Expenses over time (line), Category breakdown (pie/donut)
  - Filters: date range, category, reservation-linked
- Forms
  - Add Expense
    - Fields: Date, Amount, Currency, Category, Payment Method, Reservation (optional), Guest (optional), Description, Attachment, Recurring toggle
  - Add Income
    - Fields: Date, Amount, Currency, Source Category, Payment Method, Reservation (optional), Guest (optional), Description, Attachment
  - Category management
    - Add/Edit category with Kind (Expense/Income) and parent
  - Payment method management
    - Add/Edit method (name, provider, masked account)
- Reports
  - Financial summaries: totals by period, category, payment method
  - Trends: monthly/weekly income and expenses
  - Profit/Loss: Income minus Expenses for selected period
  - Export: CSV/PDF
- Wireframe ideas (textual)
  - Dashboard: top row KPIs with colored cards + two charts below (line & pie), filter bar at top-right
  - List pages: table with date, amount, category, reservation, method; action buttons for edit/delete
  - Forms: two-column layout; left inputs, right contextual summary (linked reservation, attachments)

## Business Logic
- Recording rules
  - Expenses reduce profit; Income increases profit
  - Validate category Kind against entry Type
  - Ensure PaymentMethod is active when used
  - Recurring entries generate instances per period (job/cron)
- Calculations
  - Profit/Loss = Sum(Income) - Sum(Expenses) over selected range
  - Budget variance = Actual - Budget per category and period
  - Currency conversion: store native amount & currency; snapshot rate for reporting normalization (if multi-currency needed)
- Reservation linkage
  - Income linked to reservation bills/payments
  - Expenses optionally linked (e.g., guest-specific service purchase)

## Integration Considerations
- Internal modules
  - Reservations: link transaction to ReservationId; include room type or guest for reports
  - Payments: reuse payment methods and status where possible
  - AuditLog: log create/update/delete
- External services
  - Payment gateways (already used): reconcile fees
  - Accounting software (QuickBooks, Xero): optional export API
  - Currency rates provider: optional for multi-currency normalization

## Security and Compliance
- RBAC: Only Admin/Manager can manage finance; Viewer role for reports
- Audit Logging for all mutations
- Protect sensitive fields; do not store full card data (PCI-DSS alignment)
- HTTPS everywhere; encrypt attachments at rest if they contain sensitive info
- GDPR: purpose limitation; retention policy; right to access/delete where applicable

## Testing and Validation
- Unit tests: profit/loss, budget variance, category validation
- Integration tests: reservation-linked income, payment method usage
- UI tests: form validation, filters, exports
- UAT: finance workflows and reconciliation

## Documentation
- Module README with setup and permissions
- ERD diagrams and sequence diagrams for recurring entries
- API specs for transactions and reports
- Admin guide: budgets, categories, export procedures
