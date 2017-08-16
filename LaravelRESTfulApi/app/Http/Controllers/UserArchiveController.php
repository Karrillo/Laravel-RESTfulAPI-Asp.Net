<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Http\Requests\ArchiveRequest;
use App\Http\Controllers\Controller;
use JWTAuth;
use App\Archive;
use JWTAuthException;

class UserArchiveController extends Controller
{
    /**
     * Store a newly created resource in storage.
     *
     * @param  \Illuminate\Http\Request  $request
     * @return \Illuminate\Http\Response
     */
    public function store(ArchiveRequest $request)
    {
        $user = JWTAuth::toUser($request->token);
        $id = $user['id'];

        $archive = new Archive;
        $archive->title = $request->input('title');
        $archive->body = $this -> encrypt_string($request->input('body'));
        $archive->user_id = $id;
        if ($archive->save()) {
            return $archive;
        }
        throw new HttpException(400, "Invalid data");
    }

    /**
     * Display the specified resource.
     *
     * @param  int  $id
     * @return \Illuminate\Http\Response
     */
    public function show($id)
    {
        if (!$id) {
           throw new HttpException(400, "Invalid id");
        }
        $archive = Archive::find($id);
        $title = $archive->title;
        $body = $archive->body;
        $archive->body = $this -> decrypt_string($body);
        return response()->json([
            $archive,
        ], 200);
    }

    /**
     * Update the specified resource in storage.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  int  $id
     * @return \Illuminate\Http\Response
     */
    public function update(ArchiveRequest $request, $id)
    {
        if (!$id) {
            throw new HttpException(400, "Invalid id");
        }
        $archive = Archive::find($id);
        $archive->title = $request->input('title');
        $archive->body = $request->input('body');
        if ($archive->save()) {
            return $archive;
        }
        throw new HttpException(400, "Invalid data");
    }

    function encrypt_string($input)
    {
        $inputlen = strlen($input);// Counts $input
        $randkey = rand(1, 9); // Gets a number between 1 and 9
 
        $i = 0;
        while ($i < $inputlen)
        {
 
            $inputchr[$i] = (ord($input[$i]) - $randkey);//convert
            $i++;
        }
 
        $encrypted = implode('.', $inputchr) . '.' . (ord($randkey)+50);

        return $encrypted;
    }

    function decrypt_string($input)
    { 
    $real = "";
    $text = explode(".", $input);// splits up the string to any array
    $count = count($text);
    $y = $count-1;// To get the key of the last bit in the array 
    //number random on encrypt_string
    $calc = $text[$y]-50;
    $randkey = chr($calc);// works out the randkey number
 
    $i = 0;
    while ($i < $y)
    {
        $array[$i] = $text[$i]+$randkey; // Works out the ascii characters actual numbers
        $real .= chr($array[$i]); //The actual decryption
        $i++;
    }
 
    return $real;
    }

    function encryptText($string, $key=5) {
    
    $result = '';
    
    for($i=0, $k= strlen($string); $i<$k; $i++) {
        $char = substr($string, $i, 1);
        $keychar = substr($key, ($i % strlen($key))-1, 1);
        $char = chr(ord($char)+ord($keychar));
        $result .= $char;
        }
        return base64_encode($result);
    }

    function decryptText($string, $key=5) {
    
    $result = '';
    $string = base64_decode($string);
    
    for($i=0,$k=strlen($string); $i< $k ; $i++) {
        $char = substr($string, $i, 1);
        $keychar = substr($key, ($i % strlen($key))-1, 1);
        $char = chr(ord($char)-ord($keychar));
        $result.=$char;
        }
        return $result;
    }

    function convert($input,$key){

    // return input unaltered if the key is blank 
    if ($key == '') { 
        return $input; 
    } 
    // remove the spaces in the key 
    $key = str_replace(' ', '', $key); 
    if (strlen($key) < 8) { 
        exit('key error'); 
    } 
    // set key length to be no more than 32 characters 
    $keyLength = strlen($key); 
    if ($keyLength > 32 ) { 
        $keyLength = 32; 
    } 

    $keyArray = array(); // key array 
    // fill key array with the bitwise AND of the ith key character and 0x1F 
    for ($i = 0; $i < $keyLength; ++$i) { 
        $keyArray[$i] = ord($key{$i}) & 0x1F; 
    } 

    // perform encryption/decryption 
    for ($i = 0, $j = 0; $i < strlen($input); ++$i) { 
        $e = ord($input{$i}); 
        // if the bitwise AND of this character and 0xE0 is non-zero 
        // set this character to the bitwise XOR of itself and the jth key element 
        // else leave this character alone 
        if ($e & 0xE0) { 
            $input{$i} = chr($e ^ $keyArray[$j]); 
        } 
        // increment j, but ensure that it does not exceed keyLength-1 
        $j = ($j + 1) % $keyLength; 
    } 
    return $input; 
    }
}
