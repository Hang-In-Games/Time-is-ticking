# 🎮 TimeBouncer 타이머 시스템 설정 가이드

## 📋 새로 추가된 파일들

### 1. SVG UI 요소
- **TimerUI.svg**: 원형 타이머 UI (진행바 + 시간 표시)
- **GameResultUI.svg**: SUCCESS/FAIL 결과 화면 UI

### 2. 스크립트 파일
- **TimerManager.cs**: 제한시간 관리 및 게임 결과 처리
- **GameManager_TimeBouncer.cs**: 통합 게임 매니저 (타이머 시스템 포함)

## 🛠️ Unity 설정 방법

### 1단계: TimerManager 설정

```
GameManager 오브젝트:
├─ GameManager_TimeBouncer (Script)
├─ TimerManager (Script) ← 새로 추가
```

**TimerManager Inspector 설정:**
```
[Timer Settings]
✅ Use Timer: true
⏰ Game Duration: 10~300초 (슬라이더로 조절 가능)
   • 10초: 초고속 게임 (극한 도전)
   • 30초: 빠른 게임 (집중력 테스트)  
   • 60초: 표준 게임 (권장)
   • 120초: 여유로운 게임
   • 300초: 장기전 게임

[UI References]
📝 Timer Text: (TMP_Text 컴포넌트 할당)
📊 Timer Progress Bar: (Image 컴포넌트 할당)
🎯 Result Panel: (GameObject 할당)
✅ Success Message: (GameObject 할당) 
❌ Fail Message: (GameObject 할당)

[Timer Visual Settings]
🟢 Normal Timer Color: White
🟡 Warning Timer Color: Yellow (30초 이하)
🔴 Danger Timer Color: Red (10초 이하)
```

### 2단계: UI 캔버스 구조

```
Canvas
├── TimerUI (Panel)
│   ├── TimerText (TMP_Text) ← "01:00"
│   ├── TimerProgressBar (Image, Fill Method: Radial)
│   └── TimerBackground (Image)
│
└── ResultUI (Panel)
    ├── ResultBackground (Image, 반투명 검정)
    ├── SuccessPanel (Panel)
    │   ├── SuccessIcon (Image)
    │   ├── SuccessText (TMP_Text) ← "SUCCESS"
    │   └── SuccessSubText (TMP_Text) ← "Time Goal Achieved!"
    │
    ├── FailPanel (Panel)
    │   ├── FailIcon (Image)
    │   ├── FailText (TMP_Text) ← "FAIL"
    │   └── FailSubText (TMP_Text) ← "Time Up! Try Again"
    │
    └── RestartHintText (TMP_Text) ← "Press R to Restart"
```

### 3단계: GameManager_TimeBouncer 설정

**Inspector 설정:**
```
[TimeBouncer Game Objects]
🎯 Paddle: (시침 오브젝트)
⚽ Ball: (공 오브젝트)

[TimeBouncer Systems]
📊 Score Manager: (ScoreManager 컴포넌트)
⏰ Timer Manager: (TimerManager 컴포넌트) ← 새로 추가

[Common Score System]
✅ Use Score System: true
🎯 Target Score: 100 (목표점수, 0이면 무제한)
```

## 🎮 게임 로직

### 타이머 시스템 흐름
```
게임 시작 → 타이머 가동 → 제한시간 체크
                              ↓
목표점수 달성 → SUCCESS    시간종료 → FAIL
     ↓                        ↓
   게임종료 ←─────────────────┘
     ↓
   결과화면 표시
     ↓
   R키로 재시작
```

### 성공/실패 조건
- **SUCCESS**: 제한시간 내에 목표점수 달성
- **FAIL**: 시간 종료 시 목표점수 미달성
- **무제한 모드**: Target Score = 0 → 시간종료 시 자동 SUCCESS

## 🎯 주요 기능

### 1. 시각적 타이머
- **원형 진행바**: 남은 시간에 따라 원형으로 감소
- **시간 표시**: MM:SS 형식으로 표시
- **색상 변화**: 
  - 정상 (흰색) → 경고 (노란색, 30초) → 위험 (빨간색, 10초)

### 2. 게임 상태 관리
```csharp
public enum GameState
{
    Ready,      // 게임 준비
    Playing,    // 게임 진행  
    Paused,     // 일시정지
    GameOver    // 게임 종료
}
```

### 3. 이벤트 시스템
```csharp
// 타이머 이벤트
TimerManager.OnTimeUp += OnTimeUp;           // 시간 종료
TimerManager.OnTimeChanged += OnTimeChanged; // 시간 변화

// 스코어 이벤트  
scoreManager.OnScoreChanged += OnScoreChanged;     // 점수 변화
scoreManager.OnTargetReached += OnTargetReached;   // 목표 달성
```

## 🔧 조작법

### 기본 조작
- **A/D 키** 또는 **←/→ 키**: 시침 회전
- **P 키**: 게임 일시정지/재개
- **R 키**: 게임 재시작
- **I 키**: Input System 재초기화

### 게임 중 자동 처리
- 목표점수 달성 시 즉시 SUCCESS 화면
- 시간 종료 시 점수 확인 후 결과 표시
- 결과 화면에서 R키로 재시작 가능

## 🎨 SVG UI 활용

### TimerUI.svg 특징
- **원형 진행바**: stroke-dasharray로 구현
- **시계 스타일**: 중심점과 원형 테두리
- **실시간 업데이트**: JavaScript로 진행률 조절 가능

### GameResultUI.svg 특징
- **조건부 표시**: success-message, fail-message 그룹
- **아이콘 포함**: 체크마크(SUCCESS), X마크(FAIL)
- **반투명 배경**: 게임 화면 오버레이

## 🔍 트러블슈팅

### 타이머가 작동하지 않는 경우
1. **TimerManager 컴포넌트** 추가 확인
2. **Use Timer = true** 설정 확인
3. **UI References** 할당 확인
4. **Target Score** 설정 확인 (0 = 무제한)

### 결과 화면이 표시되지 않는 경우
1. **Result Panel** 오브젝트 할당 확인
2. **Success/Fail Message** 오브젝트 할당 확인
3. **Canvas** 설정 확인 (Screen Space - Overlay)

### 게임이 재시작되지 않는 경우
1. **R키 입력** 확인 (Input System 사용)
2. **GameManager_TimeBouncer** 스크립트 확인
3. **RestartGame()** 메서드 호출 확인

## 📊 디버그 정보

### Inspector에서 실시간 확인 가능
```
[Game State (Runtime)]
- Current Game State: Playing/Paused/GameOver
- Current Ball Speed: 현재 공 속도
- Total Collisions: 총 충돌 횟수

[Debug Info]
- Current Time: 남은 시간 (초)
- Game Duration: 전체 게임 시간
- Time Progress: 진행률 (0~1)
```

## 🎯 최종 체크리스트

- [ ] TimerManager 컴포넌트 추가 및 설정
- [ ] UI 캔버스 구조 생성
- [ ] Timer Text, Progress Bar 할당
- [ ] Result Panel, Success/Fail Message 할당
- [ ] GameManager_TimeBouncer 스크립트 할당
- [ ] Target Score 설정 (100 권장)
- [ ] Game Duration 설정 (60초 권장)
- [ ] Ball에 "Ball" 태그 설정
- [ ] 게임 테스트 및 R키 재시작 확인

---

**제한시간 게임 모드 완성!** ⏰🎮  
이제 긴장감 넘치는 시간 제한 퐁 게임을 즐길 수 있습니다!