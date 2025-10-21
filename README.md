# RichIZ - AI 자산관리 Pro v2.0

**AI 기반 개인 자산 관리 애플리케이션** - 당신의 재무 목표 달성을 돕는 스마트한 파트너

## 🌟 주요 기능

### 💵 수입/지출 관리
- 일상적인 수입과 지출을 카테고리별로 기록
- 날짜별, 카테고리별 거래 내역 조회
- 수입/지출 통계 및 트렌드 분석

### 🏦 은행 자산 관리 **[NEW]**
- 여러 은행 계좌 통합 관리
- 입출금, 저축, 정기예금, 적금 등 다양한 계좌 유형 지원
- 금리 정보 및 잔액 추적
- 계좌별 별칭 설정

### 📈 투자 포트폴리오
- 주식, 펀드, 채권, 암호화폐, 부동산 등 다양한 투자 자산 추적
- 매입가/현재가 비교를 통한 손익 자동 계산
- 수익률 실시간 계산
- 포트폴리오 다각화 분석

### 💳 예산 관리
- 월별/카테고리별 예산 설정
- 예산 대비 지출 진행률 모니터링
- 예산 초과 시 시각적 경고 (색상 변경)
- 50/30/20 예산 규칙 적용

### 🎯 재무 목표 관리 **[NEW]**
- 비상금, 주택구매, 교육비 등 다양한 목표 설정
- 목표 달성 진행률 추적
- 목표 기한 관리 및 알림
- 월별 필요 저축액 자동 계산

### 🤖 AI 분석 센터 **[NEW]**
- **투자 포트폴리오 AI 분석**: 자산 분산, 위험도, 수익률 평가
- **소비 패턴 AI 분석**: 지출 습관 분석 및 개선 제안
- **예산 최적화 추천**: AI 기반 예산 배분 제안
- **종합 재무 AI 추천**: 맞춤형 재무 전략 제시

### 📊 리포트 및 통계
- 월별 수입/지출 추이 라인 차트
- 카테고리별 지출 분포 파이 차트
- 연도별 자산 변동 분석
- 다양한 시각화 그래프

### 🔑 라이센스 관리 시스템 **[NEW]**
- 체험판 (7일), 월간, 연간, 평생 라이센스 지원
- 하드웨어 기반 라이센스 인증
- 라이센스 키 자동 발급 및 관리

### 🛡️ 보안 및 백업 **[NEW]**
- 자동 일일 데이터 백업
- 백업 복원 기능
- AES 암호화를 통한 데이터 보호
- 30일간 백업 파일 자동 보관

### 📁 데이터 관리 **[NEW]**
- CSV 형식으로 데이터 내보내기
- Excel에서 데이터 가져오기
- 전체 데이터 일괄 내보내기
- 데이터 마이그레이션 지원

### 💱 실시간 환율 연동 **[NEW]**
- 9개 주요 통화 실시간 환율 정보
- 원화 ↔ 외화 자동 변환
- 환율 기반 자산 평가

### 🔔 알림 시스템 **[NEW]**
- 예산 초과 경고
- 목표 기한 임박 알림
- 투자 손실/수익 알림
- 라이센스 만료 알림

## 🎨 사용자 인터페이스

- 직관적이고 깔끔한 Material Design
- 반응형 레이아웃
- 다크 모드 지원 (리소스 포함)
- 고해상도 디스플레이 최적화

## 🔧 기술 스택

- **Framework**: .NET 9.0
- **UI**: WPF (Windows Presentation Foundation)
- **Database**: SQLite with Entity Framework Core 9.0
- **Architecture**: MVVM Pattern
- **Charts**: LiveCharts2
- **Security**: AES Encryption, SHA-256 Hashing
- **Libraries**:
  - CommunityToolkit.Mvvm 8.3.2
  - Microsoft.EntityFrameworkCore.Sqlite 9.0.0
  - LiveChartsCore.SkiaSharpView.WPF 2.0.0-rc3.3
  - System.Management 9.0.0

## 📦 설치 및 실행

### 필요 조건
- Windows 10/11 (x64)
- .NET 9.0 Runtime (자동 포함된 self-contained 빌드)

### EXE 파일 실행
1. `Release/RichIZ.exe` 다운로드
2. 더블클릭으로 실행
3. 초기 라이센스 활성화 (체험판 7일 무료)

### 개발 환경 설정

1. 저장소 클론
```bash
git clone https://github.com/junhuhan99/RichIZ.git
cd RichIZ
```

2. 패키지 복원
```bash
dotnet restore
```

3. 빌드
```bash
dotnet build
```

4. 실행
```bash
dotnet run
```

### EXE 파일 생성

릴리스 빌드:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

빌드된 EXE 파일 위치: `bin/Release/net9.0-windows/win-x64/publish/RichIZ.exe`

## 📖 사용 방법

### 1. 초기 설정
- 앱 실행 후 라이센스 활성화 (체험판 또는 정식 라이센스)
- 은행 계좌 등록
- 월별 예산 설정
- 재무 목표 설정

### 2. 일상 사용
- 수입/지출 기록
- 투자 자산 가격 업데이트
- AI 분석 결과 확인
- 예산 초과 알림 확인

### 3. 정기 검토
- 월말 리포트 확인
- AI 추천 사항 검토
- 예산 재조정
- 목표 진행률 확인

## 💾 데이터 저장 위치

- 데이터베이스: `%APPDATA%\RichIZ\richiz.db`
- 백업 파일: `%APPDATA%\RichIZ\Backups\`

## 🆕 v2.0 주요 업데이트

- 🤖 AI 분석 엔진 탑재
- 🏦 은행 자산 관리 기능 추가
- 🎯 재무 목표 설정 및 추적
- 🔑 라이센스 관리 시스템
- 🛡️ 자동 백업 및 데이터 암호화
- 📁 Excel 데이터 가져오기/내보내기
- 💱 실시간 환율 연동
- 🔔 스마트 알림 시스템
- 🎨 UI/UX 전면 개선
- 📊 더욱 강화된 리포트 기능
- 🌙 다크 모드 테마 지원
- ⚡ 성능 최적화

## 📄 라이센스

MIT License

## 👨‍💻 개발자

- **junhuhan99**
- GitHub: https://github.com/junhuhan99
- Repository: https://github.com/junhuhan99/RichIZ

## 🤝 기여

이슈 및 풀 리퀘스트는 언제나 환영합니다!

## 📊 시스템 요구사항

- OS: Windows 10 (1903 이상) 또는 Windows 11
- Processor: 1GHz 이상
- RAM: 512MB 이상
- Disk Space: 200MB 이상
- Display: 1280x720 이상 권장

## 🎉 특징

✅ 완전 오프라인 작동 - 인터넷 연결 불필요
✅ 개인정보 보호 - 모든 데이터는 로컬에만 저장
✅ 단일 EXE 파일 - 별도 설치 불필요
✅ 자동 백업 - 데이터 손실 방지
✅ AI 추천 - 스마트한 재무 관리
✅ 무료 체험 - 7일 무료 체험판 제공

## 📞 지원

문제가 발생하거나 질문이 있으시면:
- GitHub Issues: https://github.com/junhuhan99/RichIZ/issues

## 버전 히스토리

- **v2.0.0** (2025-10-21)
  - AI 분석 엔진 추가
  - 은행 자산 관리 기능
  - 재무 목표 관리
  - 라이센스 시스템
  - 자동 백업 및 보안 기능
  - Excel 연동
  - 환율 연동
  - 알림 시스템
  - 다크 모드 지원
  - 12개 이상의 새로운 기능 추가

- **v1.0.0** (2025-10-21)
  - 초기 릴리스
  - 수입/지출 관리 기능
  - 투자 포트폴리오 기능
  - 예산 관리 기능
  - 리포트 및 통계 기능

---

**RichIZ** - 당신의 재무 자유를 향한 첫 걸음
