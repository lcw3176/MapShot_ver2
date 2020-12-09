# MapShot_ver2
지정한 좌표를 중심으로 일정 반경 위성사진 캡쳐,
기존 프로그램 개선

## 목적
* 친구 요청으로 만듬, 업무 자동화가 목적
* 기존 업무: 넓은 반경의 위성사진을 캡쳐해야 함, 이를 그대로 찍었을 시 확대하면 사진이 뭉개져서 하나도 안보임
* 이를 해결하기 위해 각각의 확대된 사진을 찍고, 그것들을 병합해주는 프로그램

## 변경점
* WinForm -> WPF, 디자인 변경
	- 편의기능(Ctrl+C, Ctrl+V)
		+ 카카오맵 View에서 주소 복사 후 Ctrl+V 누를 시 옵션설정으로 넘어가며, 클립보드에 복사된 주소 내용이 자동 입력됨
		+ 창이 분할되며 오는 불편함을 최소화
	- 디자인
		+ 지도 탐색 View와 속성을 정하는 View를 분리
		+ 현재 진행상황을 ProgressBar로 표기와 동시에 Text로 같이 표기
		+ 사진 캡쳐 작업과 병합 작업을 별개의 진행상황으로 표기(20.12.09 수정)

* 순차처리 -> 병렬처리
	- 속도 향상
	- 10단계(약 반경 10.5km) 촬영시 기존 시간 대비 1/6 가량 절약(약 6분 => 약 1분)

* 지도 파라미터 고정 -> 지도 파라미터 설정 가능
	- 화질
		+ 유일하게 이전에 설정이 가능했던 파라미터
		+ '고화질' -> '일반화질', '초고화질' -> '고화질'로 명칭 변경
	- 범위
		+ 1~10단계 존재
		+ 숫자가 커질수록 찍히는 범위도 커짐
	- 지도타입
 		+ 6가지 (Models/MapTypeEnum.cs에 기술)
	- 세부설정
		+ 사용자가 요청한 지도위에 덧씌워지는 정보들
		+ 5.위성지도 + "교통링크, 교통노드" => 초록색 도로 라인이 표시된 위성지도 생성 

* 세부 환경설정 저장, 불러오기 기능
	- config에 저장
	- 화질, 범위, 지도타입은 선택된 콤보박스의 인덱스를 저장 
	- 세부설정은 설정별 Title(각각의 키 값, Resources/option.json에 기술) 저장

## 작동 방식
1. 사용자가 요청한 주소의 위경도를 가져옴.
2. 그 값을 기준으로 정사각형의 좌표 리스트를 만듬. 
	- 1단계: 사용자 요청 주소 1개 + 추가 자동 생성 주변 좌표 8개
	- 2단계: 요청 주소 1개 + 주변 좌표 24개 ... 
3. V-world에 해당 위경도의 Static Map 요청 후 파일로 저장.
4. 작업 완료 시 해당 파일들을 한개의 이미지 파일로 병합. 

## 사용법
#### 중심점 주소 지정
![주소 지정](https://user-images.githubusercontent.com/59993347/100979612-594ce200-3587-11eb-9121-6b848679ba5d.png)

#### 캡쳐 진행중
![작업 과정](https://user-images.githubusercontent.com/59993347/100979614-5a7e0f00-3587-11eb-8105-c28ab78f0772.png)

#### 작업 결과
![작업결과](https://user-images.githubusercontent.com/59993347/100979617-5b16a580-3587-11eb-82d7-627198a0e0b9.png)

#### 작업 결과 확대
![작업확대](https://user-images.githubusercontent.com/59993347/100979623-5ce06900-3587-11eb-9ab7-d7fdd95cf608.png)


## 수정사항
* 20.12.09
	- 아래쪽으로 내려갈수록(위도값 감소) 사진 병합시 오차가 생김(사진 밀림)
	- 모든 사진은 서울을 기준으로 짰는데, 전북에서 사진을 병합할 시 개별 사진 높이를 20만큼 줄여주면 오차 사라짐
	- 이 값을 기준으로 위도가 감소할수록 개별 사진 높이를 조금씩 줄어들게 변경.
