# ADOFAI_Auto
The "A Dance of Fire and Ice" Auto Player (Frame sync Keyboard Recorder)

# 얼불춤의 오토플레이 구현

### 서문
이전에 얼불춤 오토를 개발하려 시도했지만,<br/>
시간을 저장하는 방법을 사용하여 게임과의 오차가 매우커져 실패했던 적이 있습니다.<br/>
하지만 리버스 엔지니어링 기술을 살짝 배우고,<br/>
이 영상을 보고 영감을 받아 다시 만들게 되었습니다.<br/>
영상 링크: https://youtu.be/xKgN097y-0A

### 기본 동작원리 (키보드 입력 녹화)

1. 얼불춤 프로세스에 일정하고 빠르게 상승하는 값의 주소를 찾습니다. (프레임(Frame) 이라고 하겠습니다)<br/>
  1-1. 테스트 결과 충분히 빠르게 상승하는 값이 아니면, 오차가 매우 커져 비정상 동작합니다.<br/>

2. 글로벌 버튼 입력(모든 키 입력)을 후킹합니다. (다른프로세스의 키입력도 받을 수 있게 됩니다)<br/>

3. 얼불춤의 키입력타이밍을 바탕으로 1번에서 찾은 프레임을 이용하여 타일간의 딜레이를 계산하여 저장합니다.<br/>

4. 3번의 과정을 레벨이 끝날때 까지 반복, 모든 타일의 딜레이를 저장합니다.<br/>

5. 4번에서 저장한 프레임 딜레이를 해당타일에서 딜레이만큼 기다리고 가상 키입력을 얼불춤 프로세스에 넣습니다.<br/>

6. 레벨이 자동으로 클리어 됩니다.<br/>

요약, 키보드 누르는 타이밍을 저장해서 그대로 출력함<br/>

### 추가 기능 구현 (딜레이 저장 및 수정기능)<br/>

만약 위 기본 동작에서 만들어진 딜레이 값을 저장 후 수정이 가능하다면<br/>
이론상 해당 스테이지는 완벽한 플레이가 가능하게 됩니다.<br/>

### 사용법<br/>
|단축키|설명|
|:---:|:---:|
R | 키보드 입력 녹화
E | 저장된 녹화 데이터로 오토플레이

### 종속성<br/>
InputSimulatorPlus<br/>
NeatInput.Windows<br/>
ProcessMemoryReader<br/>
