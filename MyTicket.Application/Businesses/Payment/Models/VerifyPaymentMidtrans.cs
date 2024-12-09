namespace Project.Application.Businesses.Payment.Models
{
    public class VerifyPaymentMidtrans
    {
        public string transaction_time;
        public string transaction_status;
        public string transaction_id;
        public string status_message;
        public string status_code;
        public string signature_key;
        public string payment_type;
        public string order_id;
        public string merchant_id;
        public string masked_card;
        public string gross_amount;
        public string fraud_status;
        public string eci;
        public string currency;
        public string channel_response_message;
        public string channel_response_code;
        public string card_type;
        public string bank;
        public string approval_code;
    }
}