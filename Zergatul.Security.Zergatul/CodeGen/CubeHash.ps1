function hex($params)
{
    $i = $params[0]
    $digits = $params[1]
    if (-not $digits)
    {
        $digits = 1
    }
    return [System.Convert]::ToString($i, 16).PadLeft($digits, '0')
}

$csharpcode = @'
public static uint Add(uint a, uint b)
{
    return unchecked(a + b);
}
'@

Add-Type -Name 'Helper' -Namespace 'Lul' -MemberDefinition $csharpcode
Add-Type -Path (Join-Path $PSScriptRoot '..\bin\Debug\Zergatul.dll')

clear

function Round
{
    param
    (
        [System.UInt32[]]$State
    )

    for ($i = 0; $i -lt 16; $i++)
    {
        $State[16 + $i] = [Lul.Helper]::Add($State[$i], $State[16 + $i])
    }

    for ($i = 0; $i -lt 16; $i++)
    {
        $State[$i] = [Zergatul.BitHelper]::RotateLeft($State[$i], 7)
    }

    for ($i = 0; $i -lt 8; $i++)
    {
        [int]$j = $i + 8
        [System.UInt32]$buf = $State[$i]
        $State[$i] = $State[$j]
        $State[$j] = $buf
    }

    for ($i = 0; $i -lt 16; $i++)
    {
        $State[$i] = $State[$i] -bxor $State[$i + 16]
    }

    for ($i = 0; $i -lt 8; $i++)
    {
        [int]$i1 = 0x10 -bor (($i -band 6) -shl 1) -bor ($i -band 1)
        [int]$i2 = $i1 -bor 2
        [System.UInt32]$buf = $State[$i1]
        $State[$i1] = $State[$i2]
        $State[$i2] = $buf
    }

    for ($i = 0; $i -lt 16; $i++)
    {
        $State[16 + $i] = [Lul.Helper]::Add($State[$i], $State[16 + $i])
    }

    for ($i = 0; $i -lt 16; $i++)
    {
        $State[$i] = [Zergatul.BitHelper]::RotateLeft($State[$i], 11)
    }

    for ($i = 0; $i -lt 8; $i++)
    {
        [int]$i1 = (($i -band 4) -shl 1) -bor ($i -band 3)
        [int]$i2 = $i1 -bor 4
        [System.UInt32]$buf = $State[$i1]
        $State[$i1] = $State[$i2]
        $State[$i2] = $buf
    }

    for ($i = 0; $i -lt 16; $i++)
    {
        $State[$i] = $State[$i] -bxor $State[$i + 16]
    }

    for ($i = 0; $i -lt 8; $i++)
    {
        [int]$i1 = 0x10 -bor ($i -shl 1)
        [int]$i2 = $i1 -bor 1;
        [System.UInt32]$buf = $State[$i1]
        $State[$i1] = $State[$i2]
        $State[$i2] = $buf
    }
}

function Init
{
    param
    (
        [int]$InitRounds,
        [int]$BlockRounds,
        [int]$BlockSize,
        [int]$FinalRounds,
        [int]$HashSize
    )

    [System.UInt32[]]$state = (@(0) * 32)

    $state[0] = $HashSize
    $state[1] = $BlockSize
    $state[2] = $BlockRounds

    for ($i = 0; $i -lt $InitRounds; $i++)
    {
        Round -State $state
    }

    for ($i = 0; $i -lt 32; $i++)
    {
        Write-Output "s$(hex($i, 2)) = 0x$(hex($state[$i], 8));"
    }
}

function Create-Round
{
    $State = (@('') * 32)
    for ($i = 0; $i -lt 32; $i++)
    {
        $State[$i] = "s$(hex($i, 2))"
    }

    for ($k = 0; $k -lt 2; $k++)
    {
        for ($i = 0; $i -lt 16; $i++)
        {
            Write-Output "$($State[16 + $i]) += $($State[$i]);"
        }

        for ($i = 0; $i -lt 16; $i++)
        {
            Write-Output "$($State[$i]) = RotateLeft($($State[$i]), 7);"
        }

        for ($i = 0; $i -lt 8; $i++)
        {
            [int]$j = $i + 8
            $buf = $State[$i]
            $State[$i] = $State[$j]
            $State[$j] = $buf
        }

        for ($i = 0; $i -lt 16; $i++)
        {
            Write-Output "$($State[$i]) ^= $($State[$i + 16]);"
        }

        for ($i = 0; $i -lt 8; $i++)
        {
            [int]$i1 = 0x10 -bor (($i -band 6) -shl 1) -bor ($i -band 1)
            [int]$i2 = $i1 -bor 2
            $buf = $State[$i1]
            $State[$i1] = $State[$i2]
            $State[$i2] = $buf
        }

        for ($i = 0; $i -lt 16; $i++)
        {
            Write-Output "$($State[16 + $i]) += $($State[$i]);"
        }

        for ($i = 0; $i -lt 16; $i++)
        {
            Write-Output "$($State[$i]) = RotateLeft($($State[$i]), 11);"
        }

        for ($i = 0; $i -lt 8; $i++)
        {
            [int]$i1 = (($i -band 4) -shl 1) -bor ($i -band 3)
            [int]$i2 = $i1 -bor 4
            $buf = $State[$i1]
            $State[$i1] = $State[$i2]
            $State[$i2] = $buf
        }

        for ($i = 0; $i -lt 16; $i++)
        {
            Write-Output "$($State[$i]) ^= $($State[$i + 16]);"
        }

        for ($i = 0; $i -lt 8; $i++)
        {
            [int]$i1 = 0x10 -bor ($i -shl 1)
            [int]$i2 = $i1 -bor 1
            $buf = $State[$i1]
            $State[$i1] = $State[$i2]
            $State[$i2] = $buf
        }
    }

    #Write-Output $State
}

Init -InitRounds 160 -BlockRounds 16 -BlockSize 32 -FinalRounds 160 -HashSize 48

#Create-Round