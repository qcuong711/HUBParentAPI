namespace Api.Application.Dtos;

public class TuitionTermSummaryDto
{
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public decimal TuitionAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OffsetAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsFullyPaid { get; set; }
    public string PaymentStatus => IsFullyPaid ? "PAID" : "OUTSTANDING";
}

public class TuitionCourseDetailDto
{
    public long TransactionID { get; set; }
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string? ScheduleStudyUnitID { get; set; }
    public string? StudyUnitID { get; set; }
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public decimal? Credits { get; set; }
    public int? FeeDetailTypeID { get; set; }
    public string? FeeDetailTypeName { get; set; }
    public string? Description { get; set; }
    public decimal TuitionAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OffsetAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsTermLevel { get; set; }
    public bool IsFullyPaid { get; set; }
    public string PaymentStatus => IsFullyPaid ? "PAID" : "OUTSTANDING";
}

public class OtherFeeDto
{
    public long TransactionID { get; set; }
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public int? FeeDetailTypeID { get; set; }
    public string? FeeDetailTypeName { get; set; }
    public string? Description { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OffsetAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsFullyPaid { get; set; }
    public string PaymentStatus => IsFullyPaid ? "PAID" : "OUTSTANDING";
}

public class PaymentReceiptDto
{
    public int BillID { get; set; }
    public string StudentID { get; set; } = default!;
    public string? BillNumber { get; set; }
    public string? BillSeries { get; set; }
    public DateTime? BillDate { get; set; }
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? PaymentMethodID { get; set; }
    public string? Description { get; set; }
    public string? CollectionContent { get; set; }
    public string? EInvoiceNumber { get; set; }
    public string? EInvoiceUrl { get; set; }
}

public class OutstandingDebtDto
{
    public long TransactionID { get; set; }
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string? ScheduleStudyUnitID { get; set; }
    public string? CurriculumID { get; set; }
    public string? CurriculumName { get; set; }
    public int? FeeDetailTypeID { get; set; }
    public string? FeeDetailTypeName { get; set; }
    public string? Description { get; set; }
    public bool IsTuition { get; set; }
    public decimal AmountDue { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OffsetAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RemainingAmount { get; set; }
}

public class TuitionDeadlineDto
{
    public int DeadlineID { get; set; }
    public string StudentID { get; set; } = default!;
    public string? YearStudy { get; set; }
    public string? TermID { get; set; }
    public string? CourseID { get; set; }
    public string? FeeType { get; set; }
    public int? Round { get; set; }
    public string? TimeName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? OriginalDeadline { get; set; }
    public bool IsExtended { get; set; }
    public DateTime? ExtensionStartDate { get; set; }
    public DateTime? ExtensionEndDate { get; set; }
    public string? ExtensionReason { get; set; }
    public DateTime? EffectiveDeadline { get; set; }
    public bool IsOverdue { get; set; }
}
