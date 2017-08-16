<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Http\Requests;
use App\Http\Controllers\Controller;
use JWTAuth;
use App\Archive;
use JWTAuthException;

class ArchiveController extends Controller
{

    /**
     * Display a listing of the resource.
     *
     * @return \Illuminate\Http\Response
     */
    public function index(Request $request)
    {
        $user = JWTAuth::toUser($request->token);
        $id = $user['id'];
        $archives = Archive::where('user_id','=',$id)->get();
        if (!$archives) {
            throw new HttpException(400, "Invalid data");
        }
        return response()->json(
            $archives,
            200
        );
    }

    /**
     * Remove the specified resource from storage.
     *
     * @param  int  $id
     * @return \Illuminate\Http\Response
     */
    public function destroy($id)
    {
        if (!$id) {
            throw new HttpException(400, "Invalid id");
        }
        $archive = Archive::find($id);
        $archive->delete();
        return response()->json([
            'message' => 'archive deleted',
        ], 200);
    }

    /**
     * Display the specified resource.
     *
     * @param  int  $id
     * @return \Illuminate\Http\Response
     */
    public function showNormal($id)
    {
        if (!$id) {
           throw new HttpException(400, "Invalid id");
        }
        $archive = Archive::find($id);
        $title = $archive->title;
        $body = $archive->body;
        return response()->json([
            $archive,
        ], 200);
    }
}
