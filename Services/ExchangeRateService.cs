using System;
using System.Collections.Generic;
using System.Globalization;

namespace RichIZ.Services
{
    public class ExchangeRateService
    {
        private Dictionary<string, decimal> _exchangeRates = new();
        private DateTime _lastUpdated;

        public ExchangeRateService()
        {
            // 기본 환율 설정 (실제로는 API에서 가져와야 함)
            InitializeDefaultRates();
        }

        private void InitializeDefaultRates()
        {
            _exchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 1330.50m },   // 달러
                { "JPY", 9.85m },      // 엔화 (100엔 기준)
                { "EUR", 1450.20m },   // 유로
                { "CNY", 183.75m },    // 위안
                { "GBP", 1690.30m },   // 파운드
                { "AUD", 870.40m },    // 호주 달러
                { "CAD", 980.60m },    // 캐나다 달러
                { "CHF", 1520.80m },   // 스위스 프랑
                { "HKD", 170.25m },    // 홍콩 달러
            };
            _lastUpdated = DateTime.Now;
        }

        /// <summary>
        /// 외화를 원화로 변환
        /// </summary>
        public decimal ConvertToKRW(decimal amount, string currencyCode)
        {
            if (_exchangeRates.TryGetValue(currencyCode.ToUpper(), out var rate))
            {
                // 엔화는 100엔 기준이므로 100으로 나눔
                if (currencyCode.ToUpper() == "JPY")
                {
                    return amount * rate / 100m;
                }
                return amount * rate;
            }

            return amount; // 환율 정보가 없으면 원래 금액 반환
        }

        /// <summary>
        /// 원화를 외화로 변환
        /// </summary>
        public decimal ConvertFromKRW(decimal amountKRW, string currencyCode)
        {
            if (_exchangeRates.TryGetValue(currencyCode.ToUpper(), out var rate))
            {
                if (currencyCode.ToUpper() == "JPY")
                {
                    return amountKRW / rate * 100m;
                }
                return amountKRW / rate;
            }

            return amountKRW;
        }

        /// <summary>
        /// 환율 조회
        /// </summary>
        public decimal GetExchangeRate(string currencyCode)
        {
            return _exchangeRates.TryGetValue(currencyCode.ToUpper(), out var rate) ? rate : 0;
        }

        /// <summary>
        /// 모든 환율 조회
        /// </summary>
        public Dictionary<string, decimal> GetAllRates()
        {
            return new Dictionary<string, decimal>(_exchangeRates);
        }

        /// <summary>
        /// 환율 업데이트 시간 조회
        /// </summary>
        public DateTime GetLastUpdatedTime()
        {
            return _lastUpdated;
        }

        /// <summary>
        /// 환율 갱신 (실제로는 API 호출)
        /// </summary>
        public void UpdateExchangeRates()
        {
            // 실제로는 외부 API에서 환율 정보를 가져옴
            // 여기서는 시뮬레이션으로 약간의 변동을 줌
            var random = new Random();
            var updatedRates = new Dictionary<string, decimal>();

            foreach (var kvp in _exchangeRates)
            {
                // -2% ~ +2% 범위로 랜덤 변동
                var changePercent = (decimal)(random.NextDouble() * 4 - 2) / 100m;
                var newRate = kvp.Value * (1 + changePercent);
                updatedRates[kvp.Key] = Math.Round(newRate, 2);
            }

            _exchangeRates = updatedRates;
            _lastUpdated = DateTime.Now;
        }

        /// <summary>
        /// 통화 기호 가져오기
        /// </summary>
        public string GetCurrencySymbol(string currencyCode)
        {
            return currencyCode.ToUpper() switch
            {
                "USD" => "$",
                "JPY" => "¥",
                "EUR" => "€",
                "GBP" => "£",
                "CNY" => "¥",
                "KRW" => "₩",
                _ => currencyCode
            };
        }
    }
}
