.CODE

AESEncRound PROC state:QWORD, keys:QWORD

	movdqa [byte ptr state], xmm1
	movdqa [keys], xmm2
	aesenc xmm1, xmm2
	movdqa xmm1, [state]
	ret

AESEncRound ENDP

AESEncLastRound PROC

AESEncLastRound ENDP

AESDecRound PROC

AESDecRound ENDP

AESDecLastRound PROC

AESDecLastRound ENDP

END