# Business Engine
OpenERP 회계의 핵심 프로젝트입니다.  
회계를 프로그램으로써 빠르고 정확하게 처리하며, 회계를 분석하여 경영적 지원을 할 수 있습니다.

## Operating

클래스가 가지는 하위 필드는 모두 비공개이며, Operatable 클래스를 통하여 사용되어집니다.  
각각의 ```Manage``` 클래스들은 Operatable과는 독립적인 클래스입니다.  

> Operatable를 상속하여 사용하는 것을 권장합니다.

### Operatable
```Company``` 클래스가 상속하고 있는 클래스입니다.  
```FinanceManage```, ```ReportManage```, ```AccountCompanyManage``` 클래스를 private 필드로 가지고 있으며  
사용하기 용이하게 접근을 조금씩 풀어주는 역할을 합니다.  
아래 등장하는 클래스들은 모두 Operatable과 완전히 독립된 체계를 유지하고 있으며, 최상위 사용자는 Operatable이 되어야만 합니다.

### ReportManage
마크다운 보고서를 만드는 기본적인 클래스입니다.  

### AccountCompanyManage
거래처를 관리합니다.  
위험요소들을 관찰할 수 있습니다.

## Accounting

### FinanceManage
이것을 중심으로 다른 회계가 집행됩니다. 대부분의 필드는 비공개입니다.  
관리회계로써, 회계장부를 작성할 수 있으며, 매출과 이익을 계산합니다.  
미래 운전 자금을 계산 할 수 있으며, 위험 자산 등을 분류하여 반환하는 기능이 있습니다.  
### 예시
* 회사 현금을 사용하였을때, 분개는 아래와 같이 합니다.
1. ```InsertJournalFullPayment``` : 일시불로 처리합니다.  
2. ```InsertJournalYearInstallment``` : 월 단위 할부로 처리합니다.  
3. ```InsertJournalMonthlyInstallment``` :  년 단위 할부로 처리합니다.

그 외에도, 위의 함수들을 통해 감가상각을 예상하여 월/년 단위로 나누어서 분개할 수 있습니다.

* 상품을 판매하였을때, 매출 입력은 아래와 같이 합니다.
```AddSale``` 에 할인율, 갯수(qty), 상품(IProduct 상속자) 등을 넘겨준다면 알아서 Manage의 Book에 정리합니다.  

* 매출을 계산할때, 아래와 같이 합니다.
1. ```CalculateSalesYearly```
2. ```CalculateSalesMonthly```

위 두개의 함수로 년/월 매출을 계산할 수 있으며, 저 계산을 마친 후 ```GetCalculatedSalesProfit```를 호출하면 매출총이익을 얻을 수 있습니다.  

* 영업 이익을 계산할때, 아래와 같이 합니다.
1. ```CalculateOperatingProfitYearly```
2. ```CalculateOperatingProfitMonthly```

#### Book
회계장부의 중심입니다.  
이곳에서 판매 기록과 분개를 관리할 수 있습니다.
#### Sale
판매 사실을 나타내는 클래스입니다.  
무엇을, 언제, 입금일 등을 알 수 있습니다.  
매출의 기본 단위입니다.
#### IProduct
판매 가능한 상품들은 모두 이 인터페이스를 상속해야 합니다.  
제품을 만들기 위해 필요한 원재료들을 설정하여 넣을 수 있어서, 제조 종속성을 파악합니다.
#### Journalizing
분개로써, 돈의 목적과 그 흔적을 날짜로 맞추어 볼 수 있습니다.  
설명과 더불어 카테고리를 설정하여 더욱 자세하고 분석가능한 회계를 만들 수 있습니다.