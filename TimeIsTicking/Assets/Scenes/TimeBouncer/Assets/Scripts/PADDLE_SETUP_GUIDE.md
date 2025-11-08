# TimeBouncer - 침(Paddle) 설정 가이드

## 🎯 침 GameObject 구조

### 방법 1: Pivot을 한쪽 끝에 배치 (권장)
침의 이미지나 스프라이트의 **Pivot Point를 한쪽 끝**에 설정합니다.

```
시계 침 이미지:
┌─────────────────────┐
│                     │ ← Pivot이 여기
│═══════════════════  │
│                     │
└─────────────────────┘

회전 시:
      시계 중심
         ●
         │╲
         │ ╲  ← 침이 중심을 기준으로 회전
         │  ╲
         │   ╲
```

**Unity에서 Pivot 변경 방법:**
1. 이미지 파일 선택 (Sprite)
2. Inspector에서 `Sprite Editor` 클릭
3. Sprite 창에서 Pivot 설정:
   - Custom으로 선택
   - X: 0, Y: 0.5 (왼쪽 끝 중앙)
4. Apply

**GameObject 구조:**
```
Hierarchy:
  - ClockCenter (Empty GameObject, Position 0,0,0)
  - Paddle (침)
    └ Position: (0, 0, 0) ← 시계 중심과 같은 위치
    └ Rotation: (0, 0, 0)
    └ Pivot: 왼쪽 끝
```

### 방법 2: 자식 오브젝트로 오프셋 (복잡함)
```
Hierarchy:
  - Paddle (Empty GameObject at center)
    └ PaddleVisual (Sprite, offset)
```

## 🎮 컴포넌트 설정

### Paddle GameObject
```
Components:
├─ Transform
│  └ Position: (0, 0, 0) ← 시계 중심
│
├─ PaddleController
│  ├─ Rotation Speed: 180 (초당 180도 회전)
│  ├─ Use Keyboard Input: ✓ (플레이어용)
│  └─ Rotation Center: ClockCenter (선택사항)
│
├─ Rigidbody2D
│  ├─ Body Type: Kinematic (자동 설정됨)
│  ├─ Gravity Scale: 0
│  └─ Freeze Rotation: Off
│
├─ BoxCollider2D 또는 CapsuleCollider2D
│  └─ 침의 모양에 맞게 크기 조정
│
└─ CircleBoundary (자동 추가됨)
   ├─ Center Point: ClockCenter
   └─ Radius: 180
```

### Tag 설정
- Paddle GameObject의 Tag: **`Paddle`**

## 🎨 침 이미지 만들기

### 방법 1: Unity에서 직접 생성 (간단)
```csharp
1. GameObject > 2D Object > Sprite > Square
2. 크기 조정: Scale (100, 10, 1) ← 가로로 긴 직사각형
3. 색상: Sprite Renderer > Color: White
4. Pivot 조정 (위 참고)
```

### 방법 2: SVG 벡터 이미지
침 SVG를 만들어드릴까요? 다음과 같은 형태:
```
├─────────────────────●  ← 끝에 둥근 부분
```

## 🎮 조작 방법

### 플레이어 (키보드)
- **← 또는 A**: 반시계방향 회전
- **→ 또는 D**: 시계방향 회전

### AI (나중에 구현)
```csharp
// PaddleController의 AI용 메서드
paddleController.SetRotationInput(0.5f); // -1 ~ 1 사이 값
paddleController.SetTargetAngle(45f);    // 특정 각도로 이동
```

## 📐 물리 설정 팁

### 충돌 레이어 설정
```
Layers:
- Ball: 공 레이어
- Paddle: 패들 레이어
- Boundary: 경계 레이어

Physics2D Settings:
Ball ↔ Paddle: ✓ 충돌
Ball ↔ Boundary: ✓ 충돌
Paddle ↔ Paddle: ✗ 충돌 안함
```

### 공과 침의 상호작용
```csharp
// 침에 PhysicsMaterial2D 추가 (선택사항)
Paddle의 Collider2D:
- Material: 새로운 Physics Material 2D
  - Friction: 0 (마찰 없음)
  - Bounciness: 1 (완전 반사)
```

## 🚀 빠른 설정 체크리스트

- [ ] ClockCenter GameObject 생성 (0,0,0)
- [ ] Paddle Sprite 생성 (가로로 긴 막대)
- [ ] Paddle Sprite의 Pivot을 왼쪽 끝으로 설정
- [ ] Paddle GameObject를 (0,0,0)에 배치
- [ ] Paddle Tag를 "Paddle"로 설정
- [ ] PaddleController 스크립트 추가
- [ ] Rigidbody2D, Collider2D 추가
- [ ] GameManager의 Boundary Tags에 "Paddle" 포함 확인
- [ ] 플레이 테스트: ← → 키로 회전 확인

## 🎯 예상 결과

```
게임 화면:
           12
           |
      ╱────●────╲      ← 시계 테두리 (원)
    ╱      │      ╲
   9 ──────●────── 3   ← 중심
    ╲      │╲ Paddle  
      ╲────●─●╲─── ─╱  ← 침이 회전
           6   공
           
← → 키로 침 회전
공이 침에 튕겨나감
```

## 🔧 다음 단계

1. **공 추가**
   - Tag: "Ball"
   - Rigidbody2D: Dynamic
   - 초기 속도 설정

2. **AI 패들 추가 (나중에)**
   ```csharp
   public class AIPaddleController : MonoBehaviour
   {
       public PaddleController paddle;
       public Transform ball;
       
       void Update()
       {
           // 공을 향해 회전
           Vector2 direction = ball.position - transform.position;
           float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
           paddle.SetTargetAngle(targetAngle);
       }
   }
   ```

3. **게임 로직**
   - 공이 화면 밖으로 나가면 점수
   - 라운드 시스템
   - UI 추가
