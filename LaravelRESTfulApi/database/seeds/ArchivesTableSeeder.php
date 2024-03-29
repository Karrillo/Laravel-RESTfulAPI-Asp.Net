<?php

use Illuminate\Database\Seeder;
use App\Archive;

class ArchivesTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
      factory(Archive::class, 30)->create();
    }
}
