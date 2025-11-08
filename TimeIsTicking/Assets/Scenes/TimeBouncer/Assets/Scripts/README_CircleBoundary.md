# TimeBouncer - 원형 경계 시스템 사용 가이드

## 📋 개요
시계 테두리 내에서 공과 시침이 움직이는 퐁 게임을 위한 원형 경계 제약 시스템입니다.

## 🎯 주요 기능
- **원형 경계 제약**: 오브젝트가 시계 테두리 밖으로 나가지 않도록 제한
- **물리 반사**: 경계에 닿으면 자동으로 튕겨냄
- **태그 기반 자동 설정**: 특정 태그를 가진 오브젝트에 자동으로 제약 적용
- **Scene View 시각화**: 에디터에서 경계선을 시각적으로 확인 가능

## 🚀 설정 방법

### 1. Unity 태그 설정
Unity 에디터에서 다음 태그들을 생성하세요:
- `Ball` (공)
- `HourHand` (시침)
- `MinuteHand` (분침)

**태그 생성 방법:**
1. Unity 에디터 상단: `Edit > Project Settings > Tags and Layers`
2. `Tags` 섹션의 `+` 버튼 클릭
3. 위의 태그들을 추가

### 2. 씬 구성

#### 시계 중심 오브젝트 생성
```
Hierarchy:
  - ClockCenter (Empty GameObject)
    └ Transform: Position (0, 0, 0)
  
  - ClockBorder (시계 테두리 이미지)
    └ SVG Image 또는 Sprite
```

#### GameManager 설정
1. 빈 GameObject 생성 (이름: `GameManager`)
2. `GameManager_TimeBouncer` 스크립트 추가
3. Inspector에서 설정:
   - **Clock Center**: `ClockCenter` GameObject 할당
   - **Clock Radius**: `180` (SVG 반지름과 동일하게)
   - **Boundary Tags**: 제약을 적용할 태그 배열
   - **Enable Bounce**: 체크 (경계에서 튕김)
   - **Bounciness**: `0.8` (반사 강도)

### 3. 오브젝트 설정

#### 공 (Ball)
```
GameObject: Ball
- Tag: "Ball"
- Rigidbody2D (물리 엔진)
  - Body Type: Dynamic
  - Gravity Scale: 0 (2D 공간에서 중력 제거)
- CircleCollider2D (충돌 감지)
```

#### 시침 (HourHand)
```
GameObject: HourHand
- Tag: "HourHand"
- Rigidbody2D (선택사항)
- BoxCollider2D 또는 CapsuleCollider2D
```

## 💻 스크립트 설명

### CircleBoundary.cs
개별 오브젝트에 붙어서 원형 경계를 제약하는 컴포넌트

**주요 파라미터:**
- `centerPoint`: 원의 중심점 (Transform)
- `radius`: 시계 테두리 반지름
- `objectRadius`: 오브젝트의 반지름 (충돌 여유 공간)
- `bounceOnBoundary`: 경계 반사 활성화
- `bounciness`: 반사 계수 (0~1)

**작동 원리:**
```csharp
// 매 프레임마다 오브젝트 위치 확인
if (오브젝트가 경계 밖에 있음)
{
    1. 위치를 경계 안으로 보정
    2. 속도 벡터를 반사 (Rigidbody2D 있을 경우)
}
```

### GameManager_TimeBouncer.cs
전체 게임을 관리하고 태그 기반으로 경계 제약을 자동 설정

**주요 메서드:**
- `SetupBoundaryConstraints()`: Start 시 자동으로 태그 기반 설정
- `AddBoundaryConstraint(GameObject, float)`: 런타임에 새 오브젝트 추가

## 🎮 사용 예제

### 런타임에 공 생성하기
```csharp
public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameManager_TimeBouncer gameManager;
    
    void SpawnBall()
    {
        GameObject ball = Instantiate(ballPrefab);
        ball.tag = "Ball";
        
        // GameManager를 통해 자동으로 경계 제약 추가
        gameManager.AddBoundaryConstraint(ball, 10f);
    }
}
```

### 수동으로 CircleBoundary 추가
```csharp
void AddCustomBoundary()
{
    GameObject obj = GameObject.Find("CustomObject");
    
    CircleBoundary boundary = obj.AddComponent<CircleBoundary>();
    boundary.centerPoint = clockCenter;
    boundary.radius = 180f;
    boundary.objectRadius = 15f;
    boundary.bounceOnBoundary = true;
    boundary.bounciness = 0.9f;
}
```

## 🔍 디버깅

### Scene View 기즈모
에디터 Scene View에서 다음이 표시됩니다:
- **노란색 원**: 시계 테두리 (실제 경계선)
- **초록색 원**: 유효 경계 (오브젝트 중심이 갈 수 있는 최대 범위)
- **빨간색 구**: 현재 오브젝트 위치와 반지름

### Console 로그
GameManager가 Start 시 다음 정보를 출력:
- 각 오브젝트에 CircleBoundary 추가 확인
- 총 제약된 오브젝트 개수
- 경고: 태그를 가진 오브젝트가 없을 경우

## ⚙️ 고급 설정

### 반사 물리 조정
- **bounciness = 1.0**: 완전 탄성 충돌 (에너지 손실 없음)
- **bounciness = 0.8**: 약간의 에너지 손실 (권장)
- **bounciness = 0.5**: 큰 에너지 손실
- **bounciness = 0.0**: 경계에 붙음 (반사 없음)

### 오브젝트별 다른 설정
```csharp
// 공은 강하게 튕기고
ball.GetComponent<CircleBoundary>().bounciness = 0.95f;

// 시침은 약하게 튕김
hourHand.GetComponent<CircleBoundary>().bounciness = 0.5f;
```

## 🐛 문제 해결

### 오브젝트가 경계를 뚫고 나감
- `objectRadius` 값을 늘려보세요
- Rigidbody2D의 `Collision Detection`을 `Continuous`로 변경

### 경계에서 떨림 현상
- `bounciness` 값을 낮춰보세요 (0.7~0.8)
- Rigidbody2D의 `Linear Drag`를 약간 추가

### 태그 오브젝트를 찾지 못함
- 태그 이름 철자 확인
- 오브젝트가 씬에 있는지 확인
- Start 전에 오브젝트가 생성되었는지 확인

## 📝 체크리스트
- [ ] Unity 태그 생성 완료
- [ ] ClockCenter GameObject 생성 및 위치 설정
- [ ] GameManager GameObject 생성 및 스크립트 추가
- [ ] Clock Radius 값 설정 (SVG와 일치)
- [ ] Ball, HourHand 등 오브젝트에 태그 설정
- [ ] Rigidbody2D 컴포넌트 추가 (물리 사용 시)
- [ ] Scene View에서 기즈모 확인

## 🎯 다음 단계
1. 공의 초기 속도 설정
2. 시침/분침 회전 로직 구현
3. 점수 시스템 추가
4. UI 연동
