# Time is Ticking - 문서 디렉토리

이 디렉토리는 Time is Ticking 프로젝트의 미니게임 제작을 위한 기획 및 개발 문서를 포함합니다.

## 📁 디렉토리 구조

```
doc/
├── README.md (이 파일)
├── game-design/ - 게임 기획 문서
├── technical/ - 기술 구현 가이드
└── art-audio/ - 아트 및 오디오 리소스 가이드
```

---

## 📚 문서 목록

### 게임 기획 (game-design/)

#### [minigame-design-ideas.md](game-design/minigame-design-ideas.md)
미니게임 아이디어 및 기획 문서

**주요 내용**:
- 5가지 미니게임 아이디어
  1. 시간 역행 퍼즐 (Time Rewind Puzzle)
  2. 시계 침 블록 쌓기 (Clock Hand Stacking)
  3. 시간의 틈 찾기 (Time Glitch Hunt)
  4. 시계 기어 퍼즐 (Clock Gear Puzzle)
  5. 시간 분할 액션 (Time Split Action)
- 각 게임별 핵심 메커니즘
- 구현할 기능 목록
- 필요한 리소스 (스크립트, UI, 비주얼, 오디오)
- 기존 게임(TimeBouncer)과의 연동 방안
- 개발 우선순위 제안

**대상 독자**: 게임 디자이너, 프로듀서, 개발팀 전체

---

### 기술 문서 (technical/)

#### [minigame-technical-guide.md](technical/minigame-technical-guide.md)
미니게임 기술 구현 가이드

**주요 내용**:
- 프로젝트 구조 및 기존 시스템 분석
- 미니게임 공통 아키텍처 (Manager-Centric Design)
- Unity 6 특화 기능 활용
  - Universal Render Pipeline (URP)
  - New Input System
  - Addressables
- 타임루프 메커니즘 구현
  - 상태 저장 시스템
  - 시간 조작 컨트롤러
  - 오브젝트 스냅샷
- Physics 2D 시뮬레이션
- 성능 최적화 (오브젝트 풀링, 메모리 관리)
- 데이터 저장/로드 시스템
- 디버깅 도구 및 테스트 가이드라인

**대상 독자**: 프로그래머, 테크니컬 디자이너

---

### 아트 & 오디오 (art-audio/)

#### [minigame-asset-guide.md](art-audio/minigame-asset-guide.md)
미니게임 아트 및 오디오 리소스 가이드

**주요 내용**:
- 비주얼 스타일 가이드
  - 아트 디렉션 (테마, 분위기)
  - 색상 팔레트 (기본, 시간 상태별, 피드백용)
- 필요한 스프라이트 에셋 목록
  - 공통 UI 에셋 (시계, 버튼, 아이콘)
  - 미니게임별 스프라이트
- 이펙트 & 파티클
  - 시간 조작 이펙트
  - 게임플레이 피드백 이펙트
  - 포스트 프로세싱 효과
- 오디오 에셋 목록
  - BGM (배경음악)
  - SFX (효과음)
  - 앰비언트 사운드
- 애니메이션 가이드 (타이밍, 트랜지션)
- 에셋 제작 가이드라인 (형식, 해상도, 품질)
- Unity Import 설정
- 에셋 파일 구조
- 우선순위 에셋 리스트

**대상 독자**: 아티스트, 사운드 디자이너, UI/UX 디자이너

---

## 🎯 문서 활용 가이드

### 프로젝트 시작 단계
1. **기획 검토**: `game-design/minigame-design-ideas.md`를 읽고 구현할 미니게임 선택
2. **기술 검토**: `technical/minigame-technical-guide.md`에서 기술적 실현 가능성 확인
3. **리소스 계획**: `art-audio/minigame-asset-guide.md`에서 필요한 에셋 파악

### 프로토타입 단계
1. 선택한 미니게임의 "구현할 기능" 중 핵심 메커니즘만 구현
2. 기술 가이드의 "공통 아키텍처" 적용
3. 에셋 가이드의 "프로토타입용 최소 에셋" 사용

### 알파/베타 단계
1. 기능 완성도 향상
2. 에셋 추가 및 폴리싱
3. 성능 최적화 및 테스트

---

## 🔗 관련 문서

### 기존 프로젝트 문서
- [README.md](../README.md) - 프로젝트 개요
- [TimeBouncer 설정 가이드](../TimeIsTicking/Assets/Scenes/TimeBouncer/Assets/Scripts/QUICK_SETUP_GUIDE.md)

### 외부 참고 자료
- [Unity 6 Documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/index.html)
- [Unity URP 2D](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)

---

## 📝 문서 작성 원칙

이 문서들은 다음 원칙을 따라 작성되었습니다:

1. **명확성**: 누구나 이해할 수 있는 명확한 설명
2. **실용성**: 바로 적용 가능한 구체적 가이드
3. **일관성**: Unity 6 및 기존 프로젝트 구조와 호환
4. **확장성**: 향후 추가 미니게임 개발 시 재사용 가능

---

## 🆕 문서 업데이트

### 최근 업데이트
- 2025-11-09: 초기 문서 작성 (미니게임 기획, 기술, 아트/오디오)

### 업데이트 필요 시
1. 새로운 미니게임 아이디어 추가 → `game-design/minigame-design-ideas.md`
2. 기술 사양 변경 → `technical/minigame-technical-guide.md`
3. 아트 스타일 변경 → `art-audio/minigame-asset-guide.md`
4. 각 문서에 업데이트 날짜 기록

---

## 💡 추가 문서 제안

향후 프로젝트 진행에 따라 다음 문서 추가를 고려할 수 있습니다:

- **레벨 디자인 문서**: 각 미니게임별 구체적 레벨 설계
- **밸런싱 문서**: 난이도 곡선, 보상 체계
- **내러티브 문서**: 메인 게임 스토리와 미니게임 연계
- **QA 체크리스트**: 테스트 항목 및 버그 트래킹
- **빌드 가이드**: 플랫폼별 빌드 설정

---

## 📧 문의

문서 관련 질문이나 제안이 있다면 프로젝트 이슈 트래커를 이용해 주세요.

---

**Time is Ticking** - The clock never stops, but time repeats the same five minutes.
