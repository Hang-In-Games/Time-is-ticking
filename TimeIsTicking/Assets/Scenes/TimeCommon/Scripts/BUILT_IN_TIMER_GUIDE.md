# 🎮 GameManagerBase 내장 타이머 시스템 가이드

## 📋 구현 완료 사항

### ✅ 1. 내장 타이머 시스템
- **GameManagerBase**에 통합된 타이머 기능
- **초단위 표시**: `Time: 60s` 형식
- **시간 범위**: 10초 ~ 300초 (슬라이더 조절)
- **색상 변화**: 정상(흰색) → 경고(노란색) → 위험(빨간색)

### ✅ 2. 게임 상태 관리
```csharp
// 게임 상태 플래그
protected bool gameCompleted = false;  // 게임 완료 여부
protected bool gameCleared = false;    // 게임 클리어 여부 (성공)
```

### ✅ 3. 자동 결과 처리
- **타이머 만료** → `GAME OVER` + 빨간색 텍스트
- **목표 달성** → `SUCCESS` + 초록색 텍스트
- **자동 일시정지**: 결과 표시 시 게임 멈춤

### ✅ 4. 결과 버튼 시스템
- **성공 시**: "다음 단계" 버튼 (현재는 재시작)
- **실패 시**: "다시 시도" 버튼
- **클릭 시**: 상황에 맞는 동작 실행

## 🛠️ Unity Inspector 설정

### GameManager 오브젝트 설정
```
[Common Timer System]
✅ Use Timer System: true
⏰ Game Time Limit: 60 (10~300초 슬라이더)

[Common UI References]
📝 Timer Text: (UI Text 컴포넌트)
📱 Result Text: (UI Text 컴포넌트) 
🔘 Result Button: (UI Button 컴포넌트)

[Common Score System]
✅ Use Score System: true
🎯 Target Score: 100
```

### UI 캔버스 구조
```
Canvas
├── TimerText (Text) ← "Time: 60s"
├── ScoreText (Text) ← "Score: 0 / 100"
├── ResultText (Text) ← "SUCCESS" 또는 "GAME OVER"
└── ResultButton (Button) ← "다음 단계" 또는 "다시 시도"
```

## 🎮 게임 플로우

### 시작 → 진행 → 종료
```
게임 시작
    ↓
타이머 가동 (초단위 감소)
    ↓
┌─ 목표 달성 → SUCCESS → 다음단계/재시작
│
└─ 시간 만료 → GAME OVER → 다시시도
```

### 자동 처리 흐름
1. **게임 시작**: `StartTimer()` 자동 호출
2. **매 프레임**: 타이머 감소 + UI 업데이트
3. **목표 달성**: `OnGameCleared()` → 성공 화면
4. **시간 만료**: `OnGameOver()` → 실패 화면
5. **버튼 클릭**: 상황에 맞는 동작 실행

## 🎯 주요 기능

### 📊 실시간 UI 업데이트
```csharp
// 타이머 표시
timerText.text = $"Time: {seconds}s";

// 색상 변화
if (currentTimer <= 10f) → 빨간색 (위험)
else if (currentTimer <= 30f) → 노란색 (경고)  
else → 흰색 (정상)
```

### 🏆 자동 결과 판정
```csharp
// 성공 조건
if (currentScore >= targetScore) → OnGameCleared()

// 실패 조건  
if (currentTimer <= 0f) → OnGameOver()
```

### 🔘 동적 버튼 시스템
```csharp
// 성공 시
buttonText.text = "다음 단계";
resultText.color = Color.green;

// 실패 시
buttonText.text = "다시 시도";
resultText.color = Color.red;
```

## 🎨 시각적 피드백

### 타이머 색상 변화
- **60~31초**: 🤍 흰색 (안전)
- **30~11초**: 🟡 노란색 (주의)
- **10~0초**: 🔴 빨간색 (위험)

### 결과 텍스트 색상
- **SUCCESS**: 🟢 초록색
- **GAME OVER**: 🔴 빨간색

## 🔧 개발자 팁

### 하위 클래스에서 확장
```csharp
// TimeBouncer용 GameManager에서
protected override void OnGameCleared()
{
    base.OnGameCleared(); // 기본 처리
    
    // 추가 처리 (파티클, 사운드 등)
    PlaySuccessEffect();
}
```

### 커스텀 결과 처리
```csharp
protected override void OnNextStage()
{
    // 다음 레벨 로드
    LoadNextLevel();
    
    // 또는 재시작
    RestartGame();
}
```

## 📋 체크리스트

### Unity 설정
- [ ] GameManager에 **Use Timer System = true**
- [ ] **Game Time Limit** 설정 (권장: 60초)
- [ ] **Timer Text** UI 컴포넌트 할당
- [ ] **Result Text** UI 컴포넌트 할당
- [ ] **Result Button** UI 컴포넌트 할당
- [ ] **Target Score** 설정 (권장: 100)

### 테스트 확인
- [ ] 게임 시작 시 타이머 동작
- [ ] 초단위 카운트다운 표시
- [ ] 색상 변화 (30초, 10초)
- [ ] 목표 달성 시 SUCCESS 표시
- [ ] 시간 만료 시 GAME OVER 표시
- [ ] 버튼 클릭으로 재시작 동작

---

## 🎯 결과

**완전히 통합된 타이머 시스템!** ⏰

- ✅ **단순함**: SVG 없이 UI Text만 사용
- ✅ **자동화**: 게임 상태 자동 관리
- ✅ **유연성**: 10초~300초 자유 설정
- ✅ **직관성**: 색상으로 위험도 표시
- ✅ **확장성**: 하위 클래스에서 커스텀 가능

이제 **간단하고 효율적인 타이머 게임**이 완성되었습니다! 🚀