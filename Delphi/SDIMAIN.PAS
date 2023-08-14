unit SDIMAIN;

interface

uses Windows, Classes, Graphics, Forms, Controls, Menus,
  Dialogs, StdCtrls, Buttons, ExtCtrls, ComCtrls, ImgList, StdActns,
  ActnList, ToolWin, SysUtils;

const
  PACDRIVE_MAX_DEVICES : Integer = 16;

type
  TSDIAppForm = class(TForm)
    procedure FormShow(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

type
  PACDRIVEHANDLE = THandle;

Function PacInitialize() : Integer; stdcall; external 'PacDrive32.dll';
Procedure PacShutdown() stdcall; external 'PacDrive32.dll';
Function PacSetLEDStates(id: Integer; data: Word) : Boolean; stdcall; external 'PacDrive32.dll';

var
  SDIAppForm: TSDIAppForm;

implementation

{$R *.dfm}

procedure TSDIAppForm.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  PacSetLEDStates(0, 0);
  PacShutdown();
end;

procedure TSDIAppForm.FormShow(Sender: TObject);
var
  numdevices: Integer;
begin
  numdevices := PacInitialize();

  if (numdevices > 0) then
  begin
    PacSetLEDStates(0, StrToInt('$AAAA'))
  end;
end;

end.

