# TimeSeeker 게임 구조 및 체크리스트

## 🎯 게임 설명

TimeSeeker는 TimeBouncer를 기반으로 한 그래프 탐사 게임입니다.
- 원형 궤도(시계 테두리)에 Arc 형태의 오브젝트가 랜덤하게 생성됩니다
- 바늘(시침)을 조작하여 오브젝트를 일정 시간 추적하면 점수를 획득합니다
- 난이도에 따라 오브젝트가 고정, 회전, 불규칙하게 움직입니다

## 🎮 게임 메커니즘

### 1. 랜덤 오브젝트 생성
- **오브젝트 타입**: 점(Point), 선(Line/Arc)
- **위치**: 원형 좌표계 기반 랜덤 배치
- **생성 주기**: 난이도에 따라 조절
  - Easy: 5초마다
  - Normal: 3초마다
  - Hard: 2초마다

### 2. 오브젝트 움직임 (난이도 요소)
- **Easy**: 고정 위치 (Static)
- **Normal**: 일정한 속도로 회전 (Rotating)
- **Hard**: 불규칙 궤적 (Erratic - 진동, 랜덤 방향 전환)

### 3. 충돌 판정 및 트리거
- **판정 방식**: 바늘 각도 == 오브젝트 각도 ± 허용 오차
- **유지 시간**: 
  - Easy: 1초 이상
  - Normal: 1초
  - Hard: 0.8초
- **이벤트**: 점수 증가, 색상 변화

### 4. 점수 및 피드백 시스템
- **점수**: 점/선 성공 시 +10
- **콤보 시스템**: 연속 성공 시 추가 점수 (1.5배 배율)
- **시각적 피드백**: 오브젝트 색상 변화 (흰색 → 초록색)

### 5. 난이도 관리
- 오브젝트 생성 속도 증가
- 오브젝트 움직임 패턴 다양화
- 허용 오차 범위 축소

### 6. UI/UX 요소
- 현재 점수 표시
- 난이도 레벨 표시
- 남은 시간 표시
- 콤보 표시

## 🏗️ 전체 구조

```
Scene Hierarchy:
├─ Main Camera
│  ├─ Projection: Orthographic
│  ├─ Size: 6
│  └─ Position: (0, 0, -10)
│
├─ ClockCenter (Empty GameObject)
│  └─ Position: (0, 0, 0)
│
├─ GameManager (Empty GameObject) ⭐ 핵심!
│  └─ GameManager_TimeSeeker (Script)
│     ├─ [Scene Objects]
│     │  ├─ Clock Center: ClockCenter
│     │  ├─ Clock Border: ClockBorder
│     │  └─ Needle: Needle
│     ├─ [Clock Settings]
│     │  └─ Clock Radius: 180
│     ├─ [Needle Settings]
│     │  ├─ Needle Rotation Speed: 180
│     │  └─ Use Player Input: ✓
│     ├─ [Difficulty Settings]
│     │  └─ Current Difficulty: Easy
│     ├─ [Score Settings]
│     │  ├─ Point Score: 10
│     │  ├─ Line Score: 10
│     │  └─ Combo Multiplier: 1.5
│     └─ [Game Settings]
│        ├─ Game Time: 60
│        └─ Target Score: 100
│
├─ ClockBorder (Sprite)
│  ├─ Sprite Renderer
│  │  ├─ Sprite: ClockBorder.svg
│  │  └─ Material: SpriteMaterial_URP
│  └─ Position: (0, 0, 0)
│
├─ Needle (Sprite)
│  ├─ Position: (0, 0, 0)
│  ├─ Sprite Renderer
│  │  ├─ Sprite: ClockPaddle.svg (Pivot: 왼쪽 끝!)
│  │  └─ Material: SpriteMaterial_URP
│  ├─ Rigidbody2D
│  │  └─ Body Type: Kinematic
│  └─ NeedleController (Script) - 선택사항
│
└─ Canvas (UI)
   ├─ Score Text
   ├─ Difficulty Text
   ├─ Timer Text
   └─ Combo Text
```

## ✅ 필수 체크리스트

### 1. Scripts 준비
```
Assets/Scenes/TimeSeeker/Assets/Scripts/:
- [x] GameManager_TimeSeeker.cs (메인 게임 로직)
- [x] ArcObject.cs (Arc 오브젝트)
- [x] ArcObjectData.cs (데이터 모델)
- [x] NeedleController.cs (바늘 컨트롤러 - 선택)
- [x] ClockBorderColliderSetup.cs (TimeBouncer에서 복사)
- [x] BrightSpriteSetup.cs (TimeBouncer에서 복사)
```

### 2. Assets 준비
```
Assets/Materials/:
- [ ] SpriteMaterial_URP 생성
      Shader: Universal Render Pipeline/2D/Sprite-Unlit-Default

Assets/VectorImages/ (TimeBouncer에서 재사용):
- [ ] ClockBorder.svg
- [ ] ClockPaddle.svg (Needle로 사용)
```

### 3. Camera 설정
```
Main Camera:
- [ ] Projection: Orthographic
- [ ] Size: 6
- [ ] Position: (0, 0, -10)
```

### 4. ClockCenter 생성
```
Empty GameObject:
- [ ] 이름: "ClockCenter"
- [ ] Position: (0, 0, 0)
```

### 5. ClockBorder 생성
```
Sprite GameObject:
- [ ] 이름: "ClockBorder"
- [ ] Sprite: ClockBorder.svg
- [ ] Material: SpriteMaterial_URP
- [ ] Position: (0, 0, 0)
```

### 6. Needle 생성
```
Sprite GameObject:
- [ ] 이름: "Needle"
- [ ] Sprite: ClockPaddle.svg
- [ ] Sprite의 Pivot: (0, 0.5) ← 왼쪽 끝!
- [ ] Position: (0, 0, 0)
- [ ] Material: SpriteMaterial_URP
- [ ] Rigidbody2D 추가
      - Body Type: Kinematic
- [ ] NeedleController 스크립트 추가 (선택사항)
```

### 7. UI Canvas 생성
```
Canvas GameObject:
- [ ] Render Mode: Screen Space - Overlay
- [ ] Score Text (UI > Text)
      - Text: "Score: 0"
      - Position: 상단 왼쪽
- [ ] Difficulty Text (UI > Text)
      - Text: "Difficulty: Easy"
      - Position: 상단 중앙
- [ ] Timer Text (UI > Text)
      - Text: "Time: 60s"
      - Position: 상단 오른쪽
- [ ] Combo Text (UI > Text)
      - Text: "Combo: 1x"
      - Position: 중앙 상단
```

### 8. GameManager 설정 ⭐⭐⭐
```
Empty GameObject:
- [ ] 이름: "GameManager"
- [ ] GameManager_TimeSeeker 스크립트 추가
- [ ] Inspector에서 설정:
      [Scene Objects]
      - Clock Center: ClockCenter 드래그
      - Clock Border: ClockBorder 드래그
      - Needle: Needle 드래그
      
      [Clock Settings]
      - Clock Radius: 180
      
      [Needle Settings]
      - Needle Rotation Speed: 180
      - Use Player Input: ✓
      
      [Difficulty Settings]
      - Current Difficulty: Easy
      - Easy/Normal/Hard Settings는 자동 생성됨
      
      [Score Settings]
      - Point Score: 10
      - Line Score: 10
      - Combo Multiplier: 1.5
      
      [Game Settings]
      - Game Time: 60 (0이면 무제한)
      - Target Score: 100
      
      [UI References]
      - Score Text: Score Text 드래그
      - Difficulty Text: Difficulty Text 드래그
      - Timer Text: Timer Text 드래그
      - Combo Text: Combo Text 드래그
```

## 🎮 실행 전 최종 확인

```
Hierarchy에 있어야 할 것:
- [ ] Main Camera
- [ ] ClockCenter
- [ ] GameManager ← GameManager_TimeSeeker 스크립트 있음
- [ ] ClockBorder
- [ ] Needle
- [ ] Canvas (UI 요소들)

GameManager Inspector 확인:
- [ ] 3개 오브젝트 모두 할당됨
- [ ] Clock Center: ClockCenter
- [ ] Clock Border: ClockBorder
- [ ] Needle: Needle
- [ ] UI 텍스트들 할당됨

Console 확인 (Play 모드):
- [ ] "TimeSeeker 게임 시작 - 난이도: Easy"
- [ ] "Input System 설정 완료!"
- [ ] "오브젝트 생성 - Type: ..., Angle: ..."

Game View (Play 모드):
- [ ] A/D 또는 ← → 키로 Needle 회전됨
- [ ] Arc 오브젝트가 자동 생성됨
- [ ] Needle이 오브젝트에 겹치면 색상 변화
- [ ] 1초 유지하면 점수 획득 및 오브젝트 사라짐
- [ ] UI에 점수, 난이도, 시간, 콤보 표시됨
```

## 🔧 주요 기능 설명

### GameManager_TimeSeeker.cs
```csharp
// 핵심 메서드
Start()
  ├─ ValidateReferences()        // 오브젝트 할당 확인
  ├─ InitializeSettings()        // 난이도 설정 초기화
  ├─ CreateObjectContainer()     // 오브젝트 컨테이너 생성
  ├─ SetupInputSystem()          // Input System 설정
  └─ InitializeGame()            // 게임 초기화

Update()
  ├─ HandleNeedleInput()         // 바늘 입력 처리
  ├─ UpdateTimer()               // 타이머 업데이트
  ├─ CheckObjectSpawning()       // 오브젝트 생성 체크
  ├─ CheckNeedleCollision()      // 바늘 충돌 체크
  └─ UpdateUI()                  // UI 업데이트

SpawnRandomObject()              // 랜덤 오브젝트 생성
CheckNeedleCollision()           // 바늘과 오브젝트 충돌 감지
CollectObject()                  // 오브젝트 수집 (점수 획득)
UpdateDifficulty()               // 난이도 변경
```

### ArcObject.cs
```csharp
// 핵심 메서드
Initialize()                     // 데이터로 초기화
SetupVisuals()                   // 점/선 시각화 설정
UpdateMovement()                 // 움직임 패턴 업데이트
UpdatePosition()                 // 위치 업데이트
IsOverlappingWithNeedle()        // 바늘과 겹치는지 확인
SetActive()                      // 활성화 상태 (색상 변경)
```

## 🚀 게임 플레이 흐름

1. **게임 시작**
   - 난이도 설정 초기화 (Easy/Normal/Hard)
   - 타이머 시작 (기본 60초)
   - 첫 오브젝트 생성 대기

2. **오브젝트 생성**
   - 난이도별 주기로 랜덤 생성
   - 점 또는 선 타입 랜덤 선택
   - 랜덤 각도 및 색상
   - 난이도별 움직임 패턴 적용

3. **플레이어 조작**
   - A/D 또는 ← → 키로 바늘 회전
   - 바늘을 오브젝트에 맞춤

4. **충돌 판정**
   - 바늘이 오브젝트에 겹치면 색상 변화 (흰색 → 초록색)
   - 일정 시간(1초) 유지 필요
   - 시간 충족 시 점수 획득

5. **점수 시스템**
   - 기본 점수: 10점
   - 콤보: 연속 성공 시 1.5배 증가
   - 목표 점수 달성 또는 시간 종료 시 게임 종료

## 📊 난이도별 설정

| 설정 | Easy | Normal | Hard |
|------|------|--------|------|
| 생성 주기 | 5초 | 3초 | 2초 |
| 허용 오차 | 10도 | 7도 | 5도 |
| 유지 시간 | 1초 | 1초 | 0.8초 |
| 최대 오브젝트 | 2개 | 3개 | 4개 |
| 움직임 | 고정 | 회전 | 불규칙 |

## 🎨 확장 가능성

- 다양한 Arc 크기 및 모양
- 특수 오브젝트 (보너스, 패널티)
- 파워업 시스템
- 레벨 시스템
- 리더보드
- 사운드 이펙트
- 파티클 효과

## 🐛 자주 하는 실수

1. ❌ GameManager에 오브젝트 할당 안함 → NullReferenceException
2. ❌ Needle Sprite Pivot을 중앙에 둠 → 회전이 이상함
3. ❌ UI 텍스트 할당 안함 → UI 표시 안됨
4. ❌ Camera가 Perspective → 2D가 제대로 안보임
5. ❌ 난이도 설정이 0으로 초기화됨 → 자동 생성 확인

## 💡 팁

- Scene View에서 Gizmos를 켜면 시계 경계와 바늘 방향을 시각적으로 확인 가능
- Console에서 로그를 통해 오브젝트 생성 및 점수 획득 확인
- 난이도를 변경하려면 GameManager의 CurrentDifficulty 값을 변경
- 게임 시간을 0으로 설정하면 무제한 플레이 가능
