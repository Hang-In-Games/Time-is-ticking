# 미니게임 아트 & 오디오 리소스 가이드

## 📋 문서 개요
- **프로젝트**: Time is Ticking
- **목적**: 미니게임 제작에 필요한 시각/청각 에셋 가이드라인

---

## 🎨 비주얼 스타일 가이드

### 전체 아트 디렉션
**테마**: Timeless - 시간의 정체와 순환  
**분위기**: 미니멀, 기계적, 약간의 초현실성  
**색상**: 흑백 기조 + 강조 색상 (시간 상태별)

### 색상 팔레트

#### 기본 색상
```
메인 배경: #1a1a1a (어두운 회색)
시계 외곽: #e0e0e0 (밝은 회색)
UI 배경: #2d2d2d (중간 회색)
텍스트: #ffffff (흰색)
```

#### 시간 상태별 강조 색상
```
정상 시간 흐름: #4a90e2 (파란색)
시간 정지: #f5a623 (주황색)
시간 역행: #bd10e0 (보라색)
시간 가속: #7ed321 (초록색)
시간 분할: #50e3c2 (청록색)
```

#### 게임플레이 피드백 색상
```
성공/정답: #7ed321 (초록색)
실패/오답: #d0021b (빨간색)
경고: #f8e71c (노란색)
중립: #9b9b9b (회색)
```

---

## 🖼️ 필요한 스프라이트 에셋

### 공통 UI 에셋

#### 시계 관련
```
1. ClockFace.svg - 시계 문자판 (최소 512x512px)
   - 12시간 표시
   - 분 표시 (5분 단위)
   - 투명 배경
   
2. ClockBorder.svg - 시계 외곽선
   - 원형, 두께 8-10px
   - SVG 벡터 형식 권장
   
3. HourHand.svg - 시침 (Pivot: 왼쪽 끝)
   - 길이: 반지름의 50%
   - 두께: 6-8px
   
4. MinuteHand.svg - 분침 (Pivot: 왼쪽 끝)
   - 길이: 반지름의 70%
   - 두께: 4-6px
   
5. SecondHand.svg - 초침 (선택적)
   - 길이: 반지름의 80%
   - 두께: 2px
```

#### 버튼 & UI 요소
```
6. Button_Normal.svg - 기본 버튼 (256x64px)
7. Button_Hover.svg - 호버 상태
8. Button_Pressed.svg - 눌린 상태
9. Button_Disabled.svg - 비활성화 상태

10. Panel_Background.png - 반투명 패널 배경
11. Progress_Bar_Empty.svg - 진행 바 외곽
12. Progress_Bar_Fill.svg - 진행 바 채움
```

#### 아이콘 (64x64px)
```
13. Icon_Play.svg - 재생
14. Icon_Pause.svg - 일시정지
15. Icon_Restart.svg - 재시작
16. Icon_TimeRewind.svg - 시간 되감기
17. Icon_TimeForward.svg - 시간 빨리감기
18. Icon_TimeStop.svg - 시간 정지
19. Icon_Hint.svg - 힌트
20. Icon_Settings.svg - 설정
21. Icon_Close.svg - 닫기
22. Icon_Check.svg - 확인
23. Icon_Cross.svg - 취소
```

### 미니게임별 스프라이트

#### 1. 시간 역행 퍼즐
```
- Object_Normal_[01-05].svg - 일반 오브젝트 (5종)
- Object_Active_[01-05].svg - 활성화 오브젝트
- Object_Ghost.png - 과거/미래 상태 표시 (반투명)
- Timeline_Node.svg - 타임라인 노드
- Timeline_Connection.svg - 타임라인 연결선
- Event_Marker.svg - 이벤트 마커
```

#### 2. 시계 침 블록 쌓기
```
- Block_Type[1-5].svg - 블록 종류 (5가지)
  예: 정사각형, 직사각형, L자형, T자형, 원형
- Block_Special_Sticky.svg - 특수 블록: 점착
- Block_Special_Fixed.svg - 특수 블록: 고정
- ClockHand_Platform.svg - 블록 받침대 (시침)
- Balance_Indicator.svg - 균형 표시기
```

#### 3. 시간의 틈 찾기
```
- Scene_Base.png - 기본 씬 (1920x1080px)
- Glitch_Overlay[01-10].png - 글리치 오버레이 (10종)
- Magnifier_Frame.svg - 돋보기 프레임
- Glitch_Marker.svg - 글리치 발견 마커
- Energy_Bar_Segment.svg - 에너지 바 세그먼트
```

#### 4. 시계 기어 퍼즐
```
- Gear_Small.svg - 작은 기어 (반지름 30px)
- Gear_Medium.svg - 중간 기어 (반지름 50px)
- Gear_Large.svg - 큰 기어 (반지름 70px)
- Gear_Extra_Large.svg - 특대 기어 (반지름 100px)
- Gear_Power_Source.svg - 동력원 기어
- Gear_Slot.svg - 기어 슬롯 배경
- Connection_Line.svg - 연결선
- Rotation_Arrow.svg - 회전 방향 표시
```

#### 5. 시간 분할 액션
```
- Player_Normal.svg - 기본 플레이어
- Player_Clone_Blue.svg - 분신 1 (파란색)
- Player_Clone_Red.svg - 분신 2 (빨간색)
- Player_Clone_Green.svg - 분신 3 (초록색)
- Timeline_Track.svg - 타임라인 트랙
- Branch_Point.svg - 분기점 아이콘
- Sync_Point.svg - 동기화 지점
```

---

## ✨ 이펙트 & 파티클

### 시간 조작 이펙트

#### 파티클 시스템 설정
```
1. Time_Rewind_Particles
   - 색상: 보라색 → 흰색 그라데이션
   - 방향: 반시계 방향 소용돌이
   - 수명: 0.5-1.5초
   - 크기: 시작 0.1, 끝 0.5
   
2. Time_Stop_Particles
   - 색상: 주황색, 반짝임
   - 패턴: 방사형 확산 후 정지
   - 수명: 2초 (지속)
   
3. Time_Forward_Particles
   - 색상: 초록색 → 연두색
   - 방향: 시계 방향 빠른 흐름
   - 수명: 0.3-0.8초
```

#### 포스트 프로세싱 효과
```
Time Stop: 
- 채도 감소 (-50%)
- Vignette 강조
- 미세한 Chromatic Aberration

Time Rewind:
- 화면 왜곡 (Distortion)
- 보라색 Tint
- 잔상 효과 (Motion Blur 역방향)

Time Glitch:
- RGB 분리 효과
- 픽셀 노이즈
- 주기적 깜빡임
```

### 게임플레이 피드백 이펙트
```
4. Success_Burst
   - 색상: 노란색 → 흰색
   - 패턴: 폭발형
   - 크기: 크게 시작, 빠르게 감소
   
5. Fail_Splash
   - 색상: 빨간색 → 검은색
   - 패턴: 아래로 떨어지는 불꽃
   
6. Combo_Trail
   - 색상: 레인보우 그라데이션
   - 패턴: 궤적 따라 흐름
   
7. Object_Highlight
   - 외곽선 발광 (Outline Shader)
   - 펄스 애니메이션
```

---

## 🎵 오디오 에셋

### 음악 (BGM)

#### 공통 배경음
```
1. Menu_Theme.ogg
   - 분위기: 차분하고 신비로운
   - 템포: 느림 (60-80 BPM)
   - 악기: 피아노, 패드 신디사이저
   - 길이: 2-3분 (루프 가능)
   
2. Game_Base_Loop.ogg
   - 분위기: 미니멀, 반복적
   - 템포: 중간 (100-120 BPM)
   - 악기: 시계 틱톡 소리 + 앰비언트
   - 길이: 1-2분 (완벽한 루프)
```

#### 미니게임별 BGM
```
3. Puzzle_Theme.ogg (시간 역행 퍼즐, 기어 퍼즐)
   - 분위기: 사색적, 약간 긴장감
   - 템포: 느림 (70-90 BPM)
   
4. Action_Theme.ogg (블록 쌓기, 시간 분할)
   - 분위기: 경쾌하지만 조급함
   - 템포: 빠름 (130-150 BPM)
   - 점진적 템포 증가 (난이도 연동)
   
5. Investigation_Theme.ogg (시간의 틈 찾기)
   - 분위기: 미스터리, 탐정 느낌
   - 템포: 중간 (90-110 BPM)
```

### 효과음 (SFX)

#### UI 효과음
```
6. UI_Click.wav - 버튼 클릭 (짧고 명쾌한 클릭음)
7. UI_Hover.wav - 버튼 호버 (미묘한 톤)
8. UI_Open.wav - 패널 열기 (우슈 소리)
9. UI_Close.wav - 패널 닫기 (역재생 느낌)
10. UI_Slide.wav - 슬라이더 이동 (부드러운 스크롤)
```

#### 시간 조작 효과음
```
11. Time_Rewind.wav 
    - 테이프 역재생 소리
    - 길이: 0.5-1초
    
12. Time_Stop.wav
    - 갑작스런 정지 + 공명음
    - 길이: 1초
    
13. Time_Resume.wav
    - 톱니바퀴 맞물리는 소리
    - 길이: 0.5초
    
14. Time_Fast_Forward.wav
    - 빠르게 감기는 소리
    - 피치 증가
    
15. Time_Loop_Reset.wav
    - 종 치는 소리 + 에코
    - 길이: 2초
```

#### 시계 관련 효과음
```
16. Clock_Tick.wav - 시계 초침 소리
17. Clock_Tock.wav - 시계 초침 소리 (Tick과 번갈아)
18. Clock_Chime.wav - 시계 종소리
19. Gear_Turn.wav - 기어 회전 소리
20. Gear_Lock.wav - 기어 고정 소리
```

#### 게임플레이 효과음
```
21. Block_Drop.wav - 블록 낙하
22. Block_Land.wav - 블록 착지
23. Block_Stack.wav - 블록 쌓기 성공
24. Block_Fall.wav - 블록 무너짐
25. Bounce.wav - 공 튕김 (TimeBouncer)
26. Paddle_Hit.wav - 패들 타격

27. Glitch_Found.wav - 글리치 발견
28. Glitch_Wrong.wav - 잘못된 위치 클릭
29. Hint_Reveal.wav - 힌트 표시

30. Success_Jingle.wav - 성공 멜로디 (2-3초)
31. Fail_Jingle.wav - 실패 멜로디 (1-2초)
32. Combo_Increment.wav - 콤보 증가
33. Combo_Break.wav - 콤보 끊김
34. Level_Complete.wav - 레벨 완료 (5초)
```

#### 앰비언트 사운드
```
35. Ambient_Clock_Tower.ogg - 시계탑 소리 (루프)
36. Ambient_Time_Distortion.ogg - 시간 왜곡 노이즈 (루프)
37. Ambient_Heartbeat.ogg - 심장 박동 (긴장감 연출)
```

---

## 🎬 애니메이션 가이드

### UI 애니메이션 타이밍
```
버튼 호버: 0.1초 (Ease Out)
버튼 클릭: 0.05초 (Instant → Scale Down → Up)
패널 페이드인: 0.3초 (Ease Out)
패널 페이드아웃: 0.2초 (Ease In)
슬라이드 인: 0.4초 (Ease Out Back)
슬라이드 아웃: 0.3초 (Ease In)
```

### 게임 오브젝트 애니메이션
```
시침 회전: 부드럽게 (Ease In-Out)
블록 낙하: 중력 가속 (Ease In)
블록 착지: 바운스 효과 (Ease Out Bounce)
글리치 깜빡임: 0.1초 간격 (Step)
강조 펄스: 1초 주기 (Ping-Pong)
```

### 트랜지션 효과
```
씬 전환: 시계 와이프 (원형 확대/축소)
미니게임 진입: 시간 정지 → 화면 줌
미니게임 종료: 화면 축소 → 시간 재개
```

---

## 🛠️ 에셋 제작 가이드라인

### 스프라이트 제작
```
형식: PNG (투명 배경) 또는 SVG (벡터)
해상도: 
  - 아이콘: 64x64px, 128x128px
  - UI 요소: 256x256px, 512x512px
  - 배경: 1920x1080px (Full HD)
  - 오브젝트: 128x128px ~ 512x512px

색 공간: sRGB
비트 깊이: 32-bit (RGBA)

권장 도구:
  - Adobe Illustrator (벡터)
  - Aseprite (픽셀 아트)
  - Photoshop (래스터)
```

### 오디오 제작
```
형식: 
  - BGM: .ogg (Vorbis) - 용량 효율
  - SFX: .wav (16-bit) - 품질 우선
  
샘플레이트: 44.1 kHz
비트레이트 (ogg): 128-192 kbps

길이:
  - UI SFX: 0.05-0.2초
  - 게임 SFX: 0.2-2초
  - BGM: 1-3분 (루프)

볼륨 정규화: -6dB to -3dB (피크 방지)

권장 도구:
  - Audacity (무료, SFX 편집)
  - FL Studio (BGM 작곡)
  - Reaper (프로페셔널)
```

### Unity Import 설정

#### 스프라이트
```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Bilinear
Compression: 
  - UI: None or Low Quality
  - 게임 오브젝트: Normal Quality
Max Size: 
  - 아이콘: 256
  - 오브젝트: 512-1024
  - 배경: 2048
```

#### 오디오
```
BGM:
  - Load Type: Streaming
  - Compression Format: Vorbis
  - Quality: 70-100%
  
SFX:
  - Load Type: Decompress On Load
  - Compression Format: PCM (짧은 효과음) or ADPCM
  - Quality: 100%
```

---

## 📦 에셋 파일 구조

```
Assets/
└── MiniGames/
    ├── Common/
    │   ├── Sprites/
    │   │   ├── UI/
    │   │   │   ├── Buttons/
    │   │   │   ├── Icons/
    │   │   │   └── Panels/
    │   │   └── Clock/
    │   ├── Audio/
    │   │   ├── BGM/
    │   │   ├── SFX/
    │   │   │   ├── UI/
    │   │   │   └── Time/
    │   │   └── Ambient/
    │   ├── Materials/
    │   │   ├── SpriteMaterial_URP.mat
    │   │   └── GlitchMaterial.mat
    │   └── Prefabs/
    │       └── UI_Common.prefab
    │
    ├── TimeRewindPuzzle/
    │   ├── Sprites/
    │   ├── Audio/
    │   └── Prefabs/
    │
    ├── ClockHandStacking/
    │   ├── Sprites/
    │   ├── Audio/
    │   └── Prefabs/
    │
    ├── TimeGlitchHunt/
    │   ├── Sprites/
    │   ├── Audio/
    │   └── Prefabs/
    │
    ├── ClockGearPuzzle/
    │   ├── Sprites/
    │   ├── Audio/
    │   └── Prefabs/
    │
    └── TimeSplitAction/
        ├── Sprites/
        ├── Audio/
        └── Prefabs/
```

---

## 🎨 Sprite Atlas 설정

```
Common_UI_Atlas:
  - 모든 UI 공통 스프라이트
  - Max Size: 2048x2048
  - Format: RGBA32
  
각 미니게임별 Atlas:
  - 게임 전용 스프라이트
  - Max Size: 1024x1024
  - Format: RGBA32 or RGB24 (투명도 불필요 시)
```

---

## 🔊 오디오 믹서 구조

```
Master Mixer
├── Music (BGM)
│   ├── Volume: -10dB ~ 0dB
│   └── Effects: Lowpass Filter (옵션)
│
├── SFX (효과음)
│   ├── UI
│   │   └── Volume: -5dB
│   ├── Gameplay
│   │   └── Volume: -3dB
│   └── Time_Effects
│       └── Volume: 0dB
│       └── Effects: Reverb (시간 정지 시)
│
└── Ambient
    └── Volume: -15dB
    └── Effects: Lowpass Filter
```

---

## 📝 에셋 체크리스트

### 프로토타입 단계
- [ ] 기본 UI 스프라이트 (버튼, 패널, 아이콘 10종)
- [ ] 시계 기본 요소 (문자판, 시침, 분침)
- [ ] 플레이스홀더 BGM 1개
- [ ] 필수 SFX 5종 (클릭, 성공, 실패, 시간조작 2종)

### 알파 단계
- [ ] 완성된 UI 스프라이트 세트
- [ ] 게임별 오브젝트 스프라이트 (5-10종)
- [ ] 기본 파티클 이펙트 (3-5종)
- [ ] BGM 2-3개
- [ ] SFX 15-20종

### 베타 단계
- [ ] 모든 비주얼 에셋 완성
- [ ] 포스트 프로세싱 효과 적용
- [ ] 완성된 BGM (모든 미니게임)
- [ ] 완성된 SFX 세트 (30+ 종)
- [ ] Sprite Atlas 최적화
- [ ] 오디오 믹서 밸런싱

---

## 🌟 퀄리티 가이드라인

### 비주얼
- **일관성**: 모든 에셋이 동일한 스타일과 색상 팔레트 사용
- **가독성**: UI 요소는 최소 32px 크기에서도 식별 가능
- **확장성**: 벡터(SVG) 사용으로 다양한 해상도 지원
- **최적화**: 불필요한 디테일 제거, 적절한 압축

### 오디오
- **명확성**: 효과음은 명확하고 구분 가능해야 함
- **밸런스**: BGM과 SFX 볼륨 균형 유지
- **루프**: BGM은 자연스러운 루프 포인트 필수
- **최적화**: 적절한 압축 형식 선택

---

## 🎯 우선순위 에셋 리스트

### 프로토타입용 최소 에셋
1. 시계 외곽선 (ClockBorder.svg)
2. 시침 (HourHand.svg)
3. 기본 버튼 (Button_Normal.svg)
4. 배경 패널 (Panel_Background.png)
5. UI 클릭음 (UI_Click.wav)
6. 시간 정지음 (Time_Stop.wav)
7. 간단한 배경음악 (Game_Base_Loop.ogg)

### 1차 추가 에셋 (알파)
8-12. 주요 아이콘 5종
13-17. 미니게임 오브젝트 스프라이트
18-20. 게임플레이 SFX 3종

### 2차 추가 에셋 (베타)
나머지 모든 에셋 및 폴리싱

---

## 결론

이 가이드는 Time is Ticking 프로젝트의 미니게임 제작에 필요한 모든 시각 및 청각 리소스의 사양을 정의합니다. **일관된 아트 스타일**과 **최적화된 에셋 관리**를 통해 프로젝트의 품질과 성능을 모두 확보할 수 있습니다.

**핵심 원칙**:
1. 미니멀하고 명확한 디자인
2. 시계/시간 테마 일관성
3. 크로스 플랫폼 최적화
4. 확장 가능한 에셋 구조
5. 프로토타입 우선, 점진적 완성도 향상
