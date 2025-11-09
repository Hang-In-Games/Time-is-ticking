# TimeBouncer 게임 개발 문서

## 🎯 프로젝트 개요

**TimeBouncer**는 아날로그 시계를 테마로 한 2D 퐁(Pong) 게임입니다. 시계 테두리 내에서 Ball이 움직이며, 플레이어가 조작하는 시침(Paddle)과 상호작용하는 게임입니다.

## 🏗️ 게임 구조

### 설계 철학: 중앙 관리 방식
- **Manager-Centric Design**: GameManager가 모든 오브젝트를 직접 관리
- 개별 오브젝트에 스크립트 추가 최소화
- Inspector에서 오브젝트를 Public으로 할당하여 관리

### 충돌 처리 구조
```
1. Ball ↔ Paddle: Unity Physics Engine (CircleCollider2D ↔ BoxCollider2D)
2. Ball ↔ 시계 외곽: Unity Physics (EdgeCollider2D)
3. Ball ↔ 스코어 아이템: Trigger 감지 (OnTriggerEnter2D)
```

## 📁 파일 구조

```
TimeIsTicking/Assets/Scenes/TimeBouncer/Assets/Scripts/
├── 📄 GameManager_TimeBouncer.cs     # 메인 게임 매니저 (중앙 관리)
├── 📄 BallCollisionHandler.cs       # Ball 충돌 이벤트 헬퍼
├── 📄 ClockBorderColliderSetup.cs   # 원형 EdgeCollider2D 자동 생성
├── 📄 ScoreItemData.cs              # 스코어 아이템 데이터 모델
├── 📄 ScoreItem.cs                  # 개별 스코어 아이템 오브젝트
├── 📄 ScoreManager.cs               # 스코어 시스템 관리자
└── 📄 QUICK_SETUP_GUIDE.md          # 상세 설정 가이드
```

## ⚙️ 주요 기능

### 1. 🎮 게임 플레이
- **플레이어 조작**: A/D 또는 ←/→ 키로 시침(Paddle) 회전
- **물리 시뮬레이션**: Ball이 Paddle과 시계 외곽에서 자연스럽게 반사
- **속도 관리**: Ball 속도 유지, 감쇄율 조절, 최소/최대 속도 제한

### 2. 🏆 스코어 시스템
- **자동 아이템 스폰**: 3초마다 시계 내부 랜덤 위치에 생성
- **4가지 아이템 타입**:
  - Normal (60%): 1점 - 흰색
  - Silver (25%): 3점 - 회색  
  - Gold (10%): 5점 - 노란색
  - Bonus (5%): 10점 - 주황색
- **Trigger 충돌**: Ball이 아이템에 닿으면 자동 점수 획득

### 3. 🎨 시각적 요소
- **SVG 벡터 그래픽**: 시계 테두리, 시침, Ball 모두 SVG 사용
- **URP 2D 렌더링**: Universal Render Pipeline + 2D Renderer
- **Gizmo 시각화**: 
  - 시안색: ClockBorder 실제 크기
  - 노란색: 시계 경계 (clockRadius)
  - 초록색: 스코어 아이템 스폰 영역

### 4. ⚡ 물리 시스템
- **Linear Damping 제거**: 공기저항 없는 일정한 속도 유지
- **Physics Material**: 완전 탄성 충돌 (Bounciness: 1, Friction: 0)
- **원형 경계**: EdgeCollider2D로 완벽한 원형 충돌 처리
- **Paddle 회전 속도 반영**: 시침을 빠르게 회전하면 Ball 속도 증가

## 🎛️ 설정 파라미터

### GameManager Inspector 설정
```
[Scene Objects]
- Clock Center: 시계 중심점
- Clock Border: 시계 테두리 스프라이트  
- Paddle: 플레이어 시침
- Ball: 게임 볼

[Clock Settings]
- Clock Radius: 180 (시계 반지름)

[Paddle Settings]  
- Paddle Rotation Speed: 180 (회전 속도)
- Use Player Input: ✓ (플레이어 입력 활성화)

[Ball Settings]
- Ball Initial Speed: 300 (초기 속도)
- Ball Min Speed: 250 (최소 속도)
- Ball Max Speed: 500 (최대 속도)
- Speed Decay Rate: 0~1 (속도 감쇄율)
- Boundary Bounciness: 1.0 (경계 반발력)

[Score System]
- Use Score System: ✓ (스코어 시스템 사용)
```

### ScoreManager 설정
```
[Score Settings]
- Target Score: 100 (목표 점수, 0이면 무제한)

[Item Spawn Settings]
- Spawn Interval: 3.0 (스폰 간격, 초)
- Max Active Items: 3 (최대 동시 아이템 수)
- Spawn Radius: 150 (스폰 반지름)
- Spawn Min Radius: 50 (최소 스폰 거리)
```

## 🔧 기술적 특징

### 중앙 관리 시스템
- **단일 책임**: GameManager가 모든 로직 처리
- **Inspector 기반**: 코드 수정 없이 설정 변경
- **이벤트 시스템**: ScoreManager와 GameManager 간 이벤트 통신

### Unity Physics 활용
- **Kinematic Paddle**: 물리 영향 받지 않는 플레이어 제어
- **Dynamic Ball**: 완전한 물리 시뮬레이션
- **Trigger Items**: 충돌 감지만 하는 스코어 아이템

### 성능 최적화
- **Object Pooling**: 스코어 아이템 재활용 (향후 구현 예정)
- **자동 정리**: 충돌한 아이템 자동 제거
- **효율적 충돌 감지**: Continuous Collision Detection

## 🎮 조작법

- **A 키** 또는 **← 키**: 시침 반시계방향 회전
- **D 키** 또는 **→ 키**: 시침 시계방향 회전

## 🎯 게임 목표

1. Ball을 시침으로 튕겨서 스코어 아이템 획득
2. 다양한 아이템으로 점수 수집
3. 목표 점수 달성 또는 최고 점수 경쟁

## 🔮 향후 계획

- [ ] AI 패들 추가 (2P 모드)
- [ ] UI 시스템 (점수 표시, 메뉴)  
- [ ] 사운드 효과 및 배경음악
- [ ] 파티클 효과 (충돌, 점수 획득)
- [ ] 게임 상태 관리 (시작, 일시정지, 종료)
- [ ] 레벨 시스템 및 난이도 조절

## 📋 개발 노트

### 해결된 문제들
1. **Ball이 경계에 끼는 문제**: transform.position 직접 수정 대신 Unity Physics 사용
2. **속도 점진적 감소**: Linear Damping 제거 + 속도 유지 시스템
3. **Paddle Dynamic 문제**: Kinematic 강제 설정으로 해결
4. **복잡한 스크립트 관리**: 중앙 관리 방식으로 단순화

### 설계 원칙
- **KISS (Keep It Simple, Stupid)**: 복잡성보다 단순함 추구
- **관심사 분리**: 각 스크립트의 역할 명확히 구분  
- **Unity 친화적**: Unity의 기본 기능 최대한 활용
- **확장성**: 향후 기능 추가가 쉬운 구조

---

**개발 기간**: 2025년 11월 9일  
**Unity 버전**: 2025.x  
**렌더 파이프라인**: Universal Render Pipeline (URP)  
**개발 방식**: Manager-Centric Design Pattern